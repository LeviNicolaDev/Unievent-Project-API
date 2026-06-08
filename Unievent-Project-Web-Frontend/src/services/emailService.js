import { request } from './apiClient.js';

export async function sendAccountConfirmationEmail({ email, nome, chave }) {
  return request('/Email/enviar-email-confirmacao-conta', {
    method: 'POST',
    skipAuth: true,
    body: JSON.stringify({
      Email: email,
      Nome: nome,
      Chave: chave,
    }),
    headers: { 'Content-Type': 'application/json' },
  });
}
