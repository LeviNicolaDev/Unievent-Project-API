import AsyncStorage from "@react-native-async-storage/async-storage";
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import { useAuth } from "./AuthContext";
import {
  categoryFilterMap,
  categoryFilters as defaultCategoryFilters,
  events as fallbackEvents,
} from "../data/events";
import { certificatesApi, eventsApi, institutionsApi } from "../services/api";

const EventContext = createContext(null);

const STORAGE_KEYS = {
  favorites: "@unievent:favorites",
};

function normalizeText(value) {
  return String(value || "")
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase()
    .trim();
}

function asId(eventId) {
  return String(eventId);
}

function readStoredIds(value) {
  if (!value) return [];

  try {
    const parsed = JSON.parse(value);
    return Array.isArray(parsed) ? parsed.map(asId) : [];
  } catch {
    return [];
  }
}

function attachCertificatesToEvents(events, certificates) {
  const certificateByEventId = new Map(
    certificates.map((certificate) => [asId(certificate.eventId), certificate])
  );

  return events.map((event) => {
    const certificate = certificateByEventId.get(asId(event.id));

    if (!certificate) {
      return {
        ...event,
        hasCertificate: false,
        certificate: null,
      };
    }

    return {
      ...event,
      certificate,
      hasCertificate: Boolean(certificate.temCertificado),
    };
  });
}

export function EventProvider({ children }) {
  const { student, token } = useAuth();
  const [eventItems, setEventItems] = useState(fallbackEvents);
  const [eventsLoading, setEventsLoading] = useState(false);
  const [eventsError, setEventsError] = useState(null);
  const [favoriteIds, setFavoriteIds] = useState([]);
  const [registeredIds, setRegisteredIds] = useState([]);
  const [attendedIds, setAttendedIds] = useState([]);
  const [certificateIds, setCertificateIds] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("Todos");
  const [selectedInstitutionId, setSelectedInstitutionId] = useState("");
  const [institutionFilters, setInstitutionFilters] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [hydrated, setHydrated] = useState(false);

  const storageSuffix = useMemo(() => {
    if (student?.id) return `:${student.id}`;
    if (token) return `:${token.slice(-16)}`;
    return ":guest";
  }, [student?.id, token]);

  useEffect(() => {
    let isMounted = true;

    async function loadEvents() {
      setEventsLoading(true);
      setEventsError(null);

      try {
        if (!token) {
          setEventItems(fallbackEvents);
          setRegisteredIds([]);
          setAttendedIds([]);
          setCertificateIds([]);
          return;
        }

        const [apiEvents, apiCertificates, apiInstitutions] = await Promise.all([
          eventsApi.list(token, { instituicaoId: selectedInstitutionId }),
          certificatesApi.list(token),
          institutionsApi.listPublic().catch(() => []),
        ]);

        if (isMounted) {
          setEventItems(attachCertificatesToEvents(apiEvents, apiCertificates));
          setRegisteredIds(apiCertificates.map(c => c.eventId));
          setAttendedIds(apiCertificates.filter(c => c.presencaConfirmada).map(c => c.eventId));
          setCertificateIds(apiCertificates.filter(c => c.pdfDisponivel).map(c => c.eventId));
        }
        if (isMounted) {
          setInstitutionFilters(apiInstitutions);
        }
      } catch (error) {
        if (isMounted) {
          setEventsError(error.message);
          setEventItems(fallbackEvents);
          setRegisteredIds([]);
          setAttendedIds([]);
          setCertificateIds([]);
        }
      } finally {
        if (isMounted) {
          setEventsLoading(false);
        }
      }
    }

    loadEvents();

    return () => {
      isMounted = false;
    };
  }, [selectedInstitutionId, token]);

  useEffect(() => {
    let isMounted = true;

    async function hydrate() {
      setHydrated(false);
      setFavoriteIds([]);


      try {
        const storedFavorites = await AsyncStorage.getItem(
          `${STORAGE_KEYS.favorites}${storageSuffix}`
        );

        if (!isMounted) return;

        setFavoriteIds(readStoredIds(storedFavorites));
        // Presença, inscrições e certificados são consultados somente no backend.
      } catch {
        return null;
      } finally {
        if (isMounted) {
          setHydrated(true);
        }
      }
    }

    hydrate();

    return () => {
      isMounted = false;
    };
  }, [storageSuffix]);

  useEffect(() => {
    if (!hydrated) return;

    AsyncStorage.setItem(
      `${STORAGE_KEYS.favorites}${storageSuffix}`,
      JSON.stringify(favoriteIds)
    ).catch(() => null);
  }, [favoriteIds, hydrated, storageSuffix]);

  const getEventById = useCallback(
    (eventId) =>
      eventItems.find((event) => asId(event.id) === asId(eventId)),
    [eventItems]
  );

  const isFavorite = useCallback(
    (eventId) => favoriteIds.includes(asId(eventId)),
    [favoriteIds]
  );

  const toggleFavorite = useCallback((eventId) => {
    const id = asId(eventId);

    setFavoriteIds((current) =>
      current.includes(id)
        ? current.filter((favoriteId) => favoriteId !== id)
        : [...current, id]
    );
  }, []);

  const isRegistered = useCallback(
    (eventId) => registeredIds.includes(asId(eventId)),
    [registeredIds]
  );

  const registerForEvent = useCallback(
    async (eventId) => {
      const id = asId(eventId);

      if (!token) {
        throw new Error("Faça login como aluno para garantir seu ingresso.");
      }

      try {
        await eventsApi.register(id, token);
      } catch (error) {
        if (!normalizeText(error.message).includes("ja inscrito")) {
          throw error;
        }
      }

      setRegisteredIds((current) =>
        current.includes(id) ? current : [...current, id]
      );
    },
    [token]
  );

  const hasAttended = useCallback(
    (eventId) => attendedIds.includes(asId(eventId)),
    [attendedIds]
  );

  const hasIssuedCertificate = useCallback(
    (eventId) => certificateIds.includes(asId(eventId)),
    [certificateIds]
  );

  const filteredEvents = useMemo(() => {
    const term = normalizeText(searchTerm);
    const mappedCategories = categoryFilterMap[selectedCategory];

    return eventItems.filter((event) => {
      const matchesCategory =
        selectedCategory === "Todos" ||
        event.category === selectedCategory ||
        event.filterTags?.includes(selectedCategory) ||
        mappedCategories?.includes(event.category);

      if (!matchesCategory) return false;

      if (!term) return true;

      return [
        event.title,
        event.description,
        event.category,
        event.place,
        event.location,
      ]
        .map(normalizeText)
        .some((value) => value.includes(term));
    });
  }, [eventItems, searchTerm, selectedCategory]);

  const favoriteEvents = useMemo(
    () => eventItems.filter((event) => favoriteIds.includes(asId(event.id))),
    [eventItems, favoriteIds]
  );

  const registeredEvents = useMemo(
    () => eventItems.filter((event) => registeredIds.includes(asId(event.id))),
    [eventItems, registeredIds]
  );

  const attendedEvents = useMemo(
    () => eventItems.filter((event) => attendedIds.includes(asId(event.id))),
    [attendedIds, eventItems]
  );

  const availableCategoryFilters = useMemo(() => {
    const dynamicCategories = eventItems
      .map((event) => event.category)
      .filter(Boolean);

    return [
      "Todos",
      ...new Set([...defaultCategoryFilters.slice(1), ...dynamicCategories]),
    ];
  }, [eventItems]);

  const refreshEvents = useCallback(async () => {
    setEventsLoading(true);
    setEventsError(null);

    try {
      const [apiEvents, apiCertificates] = await Promise.all([
        eventsApi.list(token, { instituicaoId: selectedInstitutionId }),
        certificatesApi.list(token),
      ]);
      setEventItems(attachCertificatesToEvents(apiEvents, apiCertificates));
      setRegisteredIds(apiCertificates.map(c => c.eventId));
      setAttendedIds(apiCertificates.filter(c => c.presencaConfirmada).map(c => c.eventId));
      setCertificateIds(apiCertificates.filter(c => c.pdfDisponivel).map(c => c.eventId));
    } catch (error) {
      setEventsError(error.message);
      setEventItems(fallbackEvents);
      setRegisteredIds([]);
      setAttendedIds([]);
      setCertificateIds([]);
    } finally {
      setEventsLoading(false);
    }
  }, [selectedInstitutionId, token]);

  const value = useMemo(
    () => ({
      events: eventItems,
      categoryFilters: availableCategoryFilters,
      eventsError,
      eventsLoading,
      selectedCategory,
      setSelectedCategory,
      selectedInstitutionId,
      setSelectedInstitutionId,
      institutionFilters,
      searchTerm,
      setSearchTerm,
      filteredEvents,
      favoriteIds,
      favoriteEvents,
      isFavorite,
      toggleFavorite,
      registeredIds,
      registeredEvents,
      isRegistered,
      registerForEvent,
      attendedIds,
      attendedEvents,
      refreshEvents,
      hasAttended,
      certificateIds,
      hasIssuedCertificate,
      getEventById,
    }),
    [
      favoriteEvents,
      favoriteIds,
      filteredEvents,
      getEventById,
      hasAttended,
      hasIssuedCertificate,
      isFavorite,
      isRegistered,
      attendedEvents,
      attendedIds,
      availableCategoryFilters,
      certificateIds,
      eventItems,
      eventsError,
      eventsLoading,
      institutionFilters,
      registeredEvents,
      registeredIds,
      refreshEvents,
      searchTerm,
      selectedInstitutionId,
      selectedCategory,
      toggleFavorite,
      registerForEvent,
    ]
  );

  return (
    <EventContext.Provider value={value}>{children}</EventContext.Provider>
  );
}

export function useEvents() {
  const context = useContext(EventContext);

  if (!context) {
    throw new Error("useEvents must be used inside EventProvider");
  }

  return context;
}
