import { request } from "./apiClient.js";

export function getAdminDashboard() {
  return request("/admin/dashboard", { method: "GET" });
}

export function getInstituicaoDashboard() {
  return request("/instituicao/dashboard", { method: "GET" });
}
