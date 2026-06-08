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
import { certificatesApi, eventsApi } from "../services/api";

const EventContext = createContext(null);

const STORAGE_KEYS = {
  favorites: "@unievent:favorites",
  registrations: "@unievent:registrations",
  attended: "@unievent:attended",
  certificates: "@unievent:certificates",
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
        hasCertificate: Boolean(event.hasCertificate && event.certificate),
      };
    }

    return {
      ...event,
      certificate,
      hasCertificate: true,
    };
  });
}

export function EventProvider({ children }) {
  const { token } = useAuth();
  const [eventItems, setEventItems] = useState(fallbackEvents);
  const [eventsLoading, setEventsLoading] = useState(false);
  const [eventsError, setEventsError] = useState(null);
  const [favoriteIds, setFavoriteIds] = useState([]);
  const [registeredIds, setRegisteredIds] = useState([]);
  const [attendedIds, setAttendedIds] = useState([]);
  const [certificateIds, setCertificateIds] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState("Todos");
  const [searchTerm, setSearchTerm] = useState("");
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    let isMounted = true;

    async function loadEvents() {
      setEventsLoading(true);
      setEventsError(null);

      try {
        const [apiEvents, apiCertificates] = await Promise.all([
          eventsApi.list(),
          certificatesApi.list().catch(() => []),
        ]);

        if (isMounted && apiEvents.length > 0) {
          setEventItems(attachCertificatesToEvents(apiEvents, apiCertificates));
        }
      } catch (error) {
        if (isMounted) {
          setEventsError(error.message);
          setEventItems(fallbackEvents);
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
  }, []);

  useEffect(() => {
    let isMounted = true;

    async function hydrate() {
      try {
        const [
          storedFavorites,
          storedRegistrations,
          storedAttended,
          storedCertificates,
        ] = await Promise.all([
          AsyncStorage.getItem(STORAGE_KEYS.favorites),
          AsyncStorage.getItem(STORAGE_KEYS.registrations),
          AsyncStorage.getItem(STORAGE_KEYS.attended),
          AsyncStorage.getItem(STORAGE_KEYS.certificates),
        ]);

        if (!isMounted) return;

        setFavoriteIds(readStoredIds(storedFavorites));
        setRegisteredIds(readStoredIds(storedRegistrations));
        setAttendedIds(readStoredIds(storedAttended));
        setCertificateIds(readStoredIds(storedCertificates));
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
  }, []);

  useEffect(() => {
    if (!hydrated) return;

    AsyncStorage.setItem(
      STORAGE_KEYS.favorites,
      JSON.stringify(favoriteIds)
    ).catch(() => null);
  }, [favoriteIds, hydrated]);

  useEffect(() => {
    if (!hydrated) return;

    AsyncStorage.setItem(
      STORAGE_KEYS.registrations,
      JSON.stringify(registeredIds)
    ).catch(() => null);
  }, [registeredIds, hydrated]);

  useEffect(() => {
    if (!hydrated) return;

    AsyncStorage.setItem(
      STORAGE_KEYS.attended,
      JSON.stringify(attendedIds)
    ).catch(() => null);
  }, [attendedIds, hydrated]);

  useEffect(() => {
    if (!hydrated) return;

    AsyncStorage.setItem(
      STORAGE_KEYS.certificates,
      JSON.stringify(certificateIds)
    ).catch(() => null);
  }, [certificateIds, hydrated]);

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

      await eventsApi.register(id, token);

      setRegisteredIds((current) =>
        current.includes(id) ? current : [...current, id]
      );
    },
    [token]
  );

  const markEventAsAttended = useCallback(
    async (eventId) => {
      const id = asId(eventId);

      if (token) {
        await eventsApi.confirmAttendance(id, token);
      }

      setAttendedIds((current) =>
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

  const issueCertificate = useCallback(
    (eventId) => {
      const id = asId(eventId);
      const event = eventItems.find((item) => asId(item.id) === id);

      if (!attendedIds.includes(id)) {
        throw new Error(
          "Leia o QR Code do ingresso antes de emitir o certificado."
        );
      }

      if (!event?.hasCertificate || !event?.certificate) {
        throw new Error("Este evento não possui certificado associado.");
      }

      setCertificateIds((current) =>
        current.includes(id) ? current : [...current, id]
      );

      return event.certificate;
    },
    [attendedIds, eventItems]
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
        eventsApi.list(),
        certificatesApi.list().catch(() => []),
      ]);
      setEventItems(
        apiEvents.length > 0
          ? attachCertificatesToEvents(apiEvents, apiCertificates)
          : fallbackEvents
      );
    } catch (error) {
      setEventsError(error.message);
      setEventItems(fallbackEvents);
    } finally {
      setEventsLoading(false);
    }
  }, []);

  const value = useMemo(
    () => ({
      events: eventItems,
      categoryFilters: availableCategoryFilters,
      eventsError,
      eventsLoading,
      selectedCategory,
      setSelectedCategory,
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
      markEventAsAttended,
      hasAttended,
      certificateIds,
      hasIssuedCertificate,
      issueCertificate,
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
      registeredEvents,
      registeredIds,
      refreshEvents,
      searchTerm,
      selectedCategory,
      toggleFavorite,
      registerForEvent,
      markEventAsAttended,
      issueCertificate,
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
