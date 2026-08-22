import { request } from "./apiClient.js";

const USER_UNIEVENT_TYPE = "UsuarioUnievent";
const USER_SECRETARY_TYPE = "UsuarioSecretaria";
const ADMINISTRATIVE_USER_TYPES = [USER_UNIEVENT_TYPE, USER_SECRETARY_TYPE];
const ADMINISTRATIVE_ROLES = ["Admin", "Secretaria"];
const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const EMAIL_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
const ID_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

function decodeJwtPayload(token) {
  const [, payload] = String(token || "").split(".");

  if (!payload) {
    return {};
  }

  try {
    const normalizedPayload = payload
      .replace(/-/g, "+")
      .replace(/_/g, "/")
      .padEnd(Math.ceil(payload.length / 4) * 4, "=");
    const json = decodeURIComponent(
      atob(normalizedPayload)
        .split("")
        .map((char) => `%${`00${char.charCodeAt(0).toString(16)}`.slice(-2)}`)
        .join(""),
    );

    return JSON.parse(json);
  } catch {
    return {};
  }
}

function createUserFromToken(token, fallbackEmail) {
  const claims = decodeJwtPayload(token);
  const roleClaim = claims[ROLE_CLAIM] || claims.role || claims.roles;
  const roleUsuario = Array.isArray(roleClaim) ? roleClaim[0] : roleClaim;
  const emailUsuario = claims[EMAIL_CLAIM] || claims.email || fallbackEmail;
  const id = claims[ID_CLAIM] || claims.nameid || claims.sub;
  const instituicaoId = claims.instituicao_id ? Number(claims.instituicao_id) : null;
  const tipoUsuario =
    claims.tipo_usuario ||
    (roleUsuario === "Admin" && !instituicaoId ? USER_UNIEVENT_TYPE : USER_SECRETARY_TYPE);
  const statusUsuario = claims.status_usuario || claims.statusUsuario || null;

  return {
    id: id ? Number(id) : null,
    email: emailUsuario,
    emailUsuario,
    roleUsuario,
    instituicaoId,
    tipoUsuario,
    statusUsuario,
  };
}

export async function loginAdmin(email, password) {
  const response = await request("/Auth/login", {
    method: "POST",
    body: JSON.stringify({
      email,
      senha: password,
    }),
    headers: {
      "Content-Type": "application/json",
    },
  });
  const token = typeof response === "string" ? response : response?.token;

  if (!token) {
    throw new Error("Token de autenticação não recebido pela API");
  }

  const user = createUserFromToken(token, email);

  if (!isAdministrativeUser(user)) {
    throw new Error("Apenas usuários administrativos podem acessar este sistema");
  }

  return {
    token,
    user,
  };
}

export function logout() {
  // Limpar dados locais
  localStorage.removeItem("authToken");
  localStorage.removeItem("authUser");
}

export function getStoredToken() {
  return localStorage.getItem("authToken");
}

export function getStoredUser() {
  const user = localStorage.getItem("authUser");
  return user ? JSON.parse(user) : null;
}

export function isUserAdmin(user) {
  return isAdministrativeUser(user);
}

export function isUserSecretary(user) {
  return isAdministrativeUser(user);
}

export function isAdministrativeUser(user) {
  return (
    ADMINISTRATIVE_USER_TYPES.includes(user?.tipoUsuario) &&
    ADMINISTRATIVE_ROLES.includes(user?.roleUsuario)
  );
}
