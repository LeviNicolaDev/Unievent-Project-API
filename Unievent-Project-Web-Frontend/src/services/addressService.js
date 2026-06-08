import { request } from './apiClient.js';

function unwrapResponse(response) {
  return response?.data || response;
}

export async function listAddresses() {
  try {
    const response = await request('/Endereco', {
      method: 'GET',
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error('Erro ao listar endereços:', error);
    throw error;
  }
}

export async function getAddressById(id) {
  try {
    const response = await request(`/Endereco/${id}`, {
      method: 'GET',
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error(`Erro ao carregar endereço ${id}:`, error);
    throw error;
  }
}

export async function createAddress(payload) {
  try {
    const response = await request('/Endereco/CriarEndereco', {
      method: 'POST',
      body: JSON.stringify(payload),
      headers: { 'Content-Type': 'application/json' },
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error('Erro ao criar endereço:', error);
    throw error;
  }
}

export async function updateAddress(id, payload) {
  try {
    const response = await request(`/Endereco/AtualizarEndereco/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(payload),
      headers: { 'Content-Type': 'application/json' },
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error(`Erro ao atualizar endereço ${id}:`, error);
    throw error;
  }
}

export async function deleteAddress(id) {
  try {
    await request(`/Endereco/${id}`, {
      method: 'DELETE',
    });
  } catch (error) {
    console.error(`Erro ao deletar endereço ${id}:`, error);
    throw error;
  }
}
