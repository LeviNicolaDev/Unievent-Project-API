import { request } from './apiClient.js';

function unwrapResponse(response) {
  return response?.data || response;
}

function buildInstitutionFormData(payload) {
  if (payload instanceof FormData) {
    return payload;
  }

  const formData = new FormData();
  const fields = {
    EmailLogin: payload.emailLogin || payload.email,
    SenhaLogin: payload.senhaLogin || payload.password,
    Cnpj: payload.cnpj,
    EnderecoId: payload.enderecoId || payload.addressId,
  };

  Object.entries(fields).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      formData.append(key, value);
    }
  });

  const photo = payload.fotoPerfil || payload.profilePhoto;
  if (photo instanceof File) {
    formData.append('FotoPerfil', photo);
  }

  return formData;
}

export async function listInstitutions() {
  try {
    const response = await request('/Instituicao', {
      method: 'GET',
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error('Erro ao listar instituições:', error);
    throw error;
  }
}

export async function getInstitutionById(id) {
  try {
    const response = await request(`/Instituicao/${id}`, {
      method: 'GET',
    });
    return unwrapResponse(response);
  } catch (error) {
    console.error(`Erro ao carregar instituição ${id}:`, error);
    throw error;
  }
}

export async function saveInstitution(payload) {
  try {
    const isUpdate = payload.id;
    const method = isUpdate ? 'PATCH' : 'POST';
    const endpoint = isUpdate ? `/Instituicao/${payload.id}` : '/Instituicao';

    const response = await request(endpoint, {
      method,
      body: buildInstitutionFormData(payload),
    });

    return unwrapResponse(response);
  } catch (error) {
    console.error('Erro ao salvar instituição:', error);
    throw error;
  }
}

export async function deleteInstitution(id) {
  try {
    await request(`/Instituicao/${id}`, {
      method: 'DELETE',
    });
  } catch (error) {
    console.error(`Erro ao deletar instituição ${id}:`, error);
    throw error;
  }
}
