import { request } from "./apiClient.js";

const USER_SECRETARY_TYPE = "UsuarioSecretaria";
const USER_SECRETARY_ROLES = ["Admin", "Secretaria"];
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

  return {
    id: id ? Number(id) : null,
    email: emailUsuario,
    emailUsuario,
    roleUsuario,
    tipoUsuario: USER_SECRETARY_TYPE,
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

  if (!isUserSecretary(user)) {
    throw new Error("Apenas usuários da secretaria podem acessar este sistema");
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
  return isUserSecretary(user);
}

export function isUserSecretary(user) {
  return (
    user?.tipoUsuario === USER_SECRETARY_TYPE &&
    USER_SECRETARY_ROLES.includes(user?.roleUsuario)
  );
}
