import { Image, Platform } from "react-native";

import defaultStudentPhoto from "../../assets/icon.png";
import { events as fallbackEvents } from "../data/events";

const FALLBACK_API_ORIGIN =
  Platform.OS === "android" ? "http://10.0.2.2:5227" : "http://localhost:5227";

function normalizeApiOrigin(value) {
  const configuredOrigin = value?.replace(/\/api\/?$/, "").replace(/\/$/, "");

  if (!configuredOrigin) return "";

  if (
    Platform.OS === "android" &&
    /^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/i.test(configuredOrigin)
  ) {
    return configuredOrigin.replace(
      /\/\/(localhost|127\.0\.0\.1)/i,
      "//10.0.2.2"
    );
  }

  return configuredOrigin;
}

export const API_ORIGIN =
  normalizeApiOrigin(process.env.EXPO_PUBLIC_API_URL) || FALLBACK_API_ORIGIN;

export const API_URL = `${API_ORIGIN}/api`;

class ApiError extends Error {
  constructor(message, status, details) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

function normalizeErrorMessage(data, fallback) {
  if (!data) return fallback;
  if (typeof data === "string") return data;
  if (Array.isArray(data)) return data.join("\n");
  if (typeof data.message === "string") return data.message;
  if (typeof data.title === "string") return data.title;

  const values = Object.values(data).flat();
  if (values.length > 0 && values.every((value) => typeof value === "string")) {
    return values.join("\n");
  }

  return fallback;
}

async function parseResponse(response) {
  const text = await response.text();
  if (!text) return null;

  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

async function apiFetch(path, options = {}) {
  const {
    body,
    headers = {},
    method = "GET",
    token,
    multipart = body instanceof FormData,
  } = options;
  const requestHeaders = {
    Accept: "application/json",
    ...headers,
  };

  if (token) {
    requestHeaders.Authorization = `Bearer ${token}`;
  }

  let requestBody = body;
  if (body && !multipart) {
    requestHeaders["Content-Type"] = "application/json";
    requestBody = JSON.stringify(body);
  }

  const response = await fetch(`${API_URL}${path}`, {
    method,
    headers: requestHeaders,
    body: requestBody,
  });
  const data = await parseResponse(response);

  if (!response.ok) {
    throw new ApiError(
      normalizeErrorMessage(data, "Não foi possível completar a solicitação."),
      response.status,
      data
    );
  }

  return data;
}

async function getDefaultPhotoFile() {
  if (Platform.OS === "web") {
    const base64 =
      "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAFgwJ/lbBv9wAAAABJRU5ErkJggg==";
    const bytes = Uint8Array.from(atob(base64), (char) => char.charCodeAt(0));
    return new Blob([bytes], { type: "image/png" });
  }

  const source = Image.resolveAssetSource(defaultStudentPhoto);

  return {
    uri: source.uri,
    name: "perfil.png",
    type: "image/png",
  };
}

function appendPhoto(formData, photo) {
  if (Platform.OS === "web" && photo instanceof Blob) {
    formData.append("FotoPerfil", photo, photo.name || "perfil.png");
    return;
  }

  formData.append("FotoPerfil", photo);
}

async function buildStudentCreateForm(data) {
  const formData = new FormData();
  const photo = data.fotoPerfil || (await getDefaultPhotoFile());

  formData.append("Nome", data.nome.trim());
  formData.append("Email", data.email.trim());
  formData.append("Senha", data.senha);
  formData.append("DataNascimento", data.dataNascimento);
  formData.append("TipoParticipante", "Interno");
  formData.append("InstituicaoId", data.instituicaoId);
  appendPhoto(formData, photo);

  return formData;
}

async function buildStudentUpdateForm(data) {
  const formData = new FormData();

  if (data.nome?.trim()) {
    formData.append("Nome", data.nome.trim());
  }

  if (data.senha) {
    formData.append("Senha", data.senha);
  }

  if (data.dataNascimento) {
    formData.append("DataNascimento", data.dataNascimento);
  }

  if (data.fotoPerfil) {
    appendPhoto(formData, data.fotoPerfil);
  }

  return formData;
}

export function getApiAssetUrl(value) {
  if (!value) return null;
  if (/^https?:\/\//i.test(value)) return value;

  return `${API_ORIGIN}${value.startsWith("/") ? "" : "/"}${value}`;
}

function getBestThumbnail(thumbnail) {
  const values = (Array.isArray(thumbnail) ? thumbnail : [thumbnail]).filter(
    Boolean
  );

  if (values.length === 0) return null;

  return (
    values.find((value) =>
      String(value).toLowerCase().includes("original")
    ) || values[values.length - 1]
  );
}

function formatCategory(value) {
  const normalized = String(value || "Evento").toLowerCase();
  const labels = {
    palestra: "Palestras",
    workshop: "Workshop",
    feira: "Feiras",
    evento: "Eventos",
    festa: "Festas",
  };

  return labels[normalized] || String(value || "Evento");
}

function normalizeAudience(value) {
  const normalized = String(value || "PublicoGeral").toLowerCase();
  const map = {
    alunosdainstituicao: "AlunosDaInstituicao",
    todosalunosfatec: "TodosAlunosFatec",
    publicogeral: "PublicoGeral",
  };

  return map[normalized] || String(value || "PublicoGeral");
}

function formatAudience(value, institutionName) {
  const normalized = normalizeAudience(value);
  const labels = {
    AlunosDaInstituicao: institutionName
      ? `Somente alunos da ${institutionName}`
      : "Somente alunos desta instituição",
    TodosAlunosFatec: "Todos os alunos FATEC",
    PublicoGeral: "Público geral",
  };

  return labels[normalized] || normalized;
}

function formatDateParts(value) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return {
      date: "Data a definir",
      fullDate: "Data a definir",
      time: "--h",
      displayTime: "Horário a definir",
    };
  }

  const dateLabel = new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(date);
  const timeLabel = new Intl.DateTimeFormat("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);

  return {
    date: dateLabel.replace(" de ", " ").replace(" de ", ", "),
    fullDate: dateLabel,
    time: timeLabel.replace(":00", "h"),
    displayTime: timeLabel,
  };
}

export function mapApiEvent(event, index = 0) {
  const category = formatCategory(event.categoria ?? event.Categoria);
  const thumbnail = event.thumbnail ?? event.Thumbnail ?? [];
  const imageUri = getApiAssetUrl(getBestThumbnail(thumbnail));
  const fallback = fallbackEvents[index % fallbackEvents.length];
  const dates = formatDateParts(event.dataEvento ?? event.DataEvento);
  const title = event.nome ?? event.Nome ?? fallback.title;
  const local = event.local ?? event.Local ?? "";
  const institutionName =
    event.instituicaoNome ??
    event.InstituicaoNome ??
    "Instituição não informada";
  const publicoPermitido =
    normalizeAudience(
      event.publicoPermitido ??
      event.PublicoPermitido ??
      "PublicoGeral"
    );
  const certificate =
    event.certificado ??
    event.Certificado ??
    event.certificate ??
    event.Certificate ??
    null;
  const explicitHasCertificate =
    event.hasCertificate ??
    event.HasCertificate ??
    event.temCertificado ??
    event.TemCertificado;

  return {
    id: String(event.id ?? event.Id),
    title,
    description: event.descricao ?? event.Descricao ?? "",
    place: event.responsavel ?? event.Responsavel ?? fallback.place,
    location: local || fallback.location,
    local,
    category,
    filterTags: [category],
    image: imageUri ? { uri: imageUri, cache: "force-cache" } : fallback.image,
    price: 0,
    isFree: true,
    vacancies: event.capacidade ?? event.Capacidade ?? 0,
    organizer: event.responsavel ?? event.Responsavel ?? fallback.organizer,
    institutionId: event.instituicaoId ?? event.InstituicaoId ?? null,
    institutionName,
    publicoPermitido,
    audience: publicoPermitido,
    audienceLabel: formatAudience(publicoPermitido, institutionName),
    hasCertificate:
      typeof explicitHasCertificate === "boolean"
        ? explicitHasCertificate
        : Boolean(certificate),
    certificate,
    apiEvent: event,
    ...dates,
  };
}

function normalizeInstitution(institution) {
  const id = institution.id ?? institution.Id ?? null;

  return {
    id,
    codigo: institution.codigo ?? institution.Codigo ?? "",
    nome:
      institution.nome ??
      institution.Nome ??
      institution.nomeAbreviado ??
      institution.NomeAbreviado ??
      "",
    cidade: institution.cidade ?? institution.Cidade ?? "",
    estado: institution.estado ?? institution.Estado ?? "",
  };
}

export function mapApiCertificate(certificate) {
  return {
    ...certificate,
    id: String(certificate.id ?? certificate.Id),
    eventId: String(certificate.eventoId ?? certificate.EventoId),
    date:
      certificate.dataCertifcado ??
      certificate.DataCertifcado ??
      certificate.dataCertificado ??
      certificate.DataCertificado,
    text: certificate.texto ?? certificate.Texto ?? "",
    eventName: certificate.nomeEvento ?? certificate.NomeEvento ?? "",
    apiCertificate: certificate,
  };
}

export const authApi = {
  loginAluno(credentials) {
    return apiFetch("/Auth/login-aluno", {
      method: "POST",
      body: {
        email: credentials.email.trim(),
        senha: credentials.senha,
      },
    });
  },
};

export const studentsApi = {
  async create(data) {
    return apiFetch("/Aluno", {
      method: "POST",
      body: await buildStudentCreateForm(data),
      multipart: true,
    });
  },
  list() {
    return apiFetch("/Aluno");
  },
  getById(id) {
    return apiFetch(`/Aluno/${id}`);
  },
  getMe(token) {
    return apiFetch("/Aluno/me", { token });
  },
  async update(id, data, token) {
    return apiFetch(`/Aluno/${id}`, {
      method: "PATCH",
      body: await buildStudentUpdateForm(data),
      multipart: true,
      token,
    });
  },
};

export const eventsApi = {
  async list(token, filters = {}) {
    const query = new URLSearchParams({
      Page: "1",
      PageSize: "50",
    });

    if (filters.instituicaoId) {
      query.set("InstituicaoId", filters.instituicaoId);
    }

    const result = await apiFetch(`/Evento?${query.toString()}`, { token });
    const items = Array.isArray(result)
      ? result
      : result?.items ?? result?.Items ?? [];
    return Array.isArray(items) ? items.map(mapApiEvent) : [];
  },
  register(eventId, token) {
    return apiFetch(`/Evento/${eventId}/inscrever-se`, {
      method: "POST",
      token,
    });
  },
  getTicket(eventId, token) {
    return apiFetch(`/Evento/${eventId}/ingresso`, { token });
  },
  confirmAttendance(eventId, token) {
    throw new Error("A presença deve ser validada por um operador credenciado.");
  },
};

export const institutionsApi = {
  async listPublic() {
    const result = await apiFetch("/Instituicao/ativas");
    const items = Array.isArray(result)
      ? result
      : result?.items ?? result?.Items ?? [];

    return Array.isArray(items)
      ? items.map(normalizeInstitution).filter((institution) => institution.id)
      : [];
  },
};

export const certificatesApi = {
  async list(token) {
    const result = await apiFetch("/Certificado/meus", { token });
    return Array.isArray(result) ? result.map(mapApiCertificate) : [];
  },
};

export const preferencesApi = {
  get(token) {
    return apiFetch('/me/preferencias', { token });
  },
  update(token, preferences) {
    return apiFetch('/me/preferencias', { method: 'PUT', token, body: preferences });
  },
};
