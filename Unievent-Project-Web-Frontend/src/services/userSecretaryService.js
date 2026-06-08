import { request } from './apiClient.js';

function unwrapResponse(response) {
  return response?.data || response;
}

function normalizeUserSecretary(user) {
  if (!user) {
    return null;
  }

  return {
    ...user,
    tipoUsuario: 'UsuarioSecretaria',
    nome: user.nomeUsuario || user.nome,
    email: user.emailUsuario || user.email,
  };
}

function toUserSecretaryPayload(payload) {
  if (payload.id) {
    return {
      NomeUsuario: payload.nomeUsuario || payload.nome || payload.name,
      EmailUsuario: payload.emailUsuario || payload.email,
      Senha: payload.senha || payload.password,
      Role: payload.role || payload.roleUsuario,
    };
  }

  return {
    NomeUsuario: payload.nomeUsuario || payload.nome || payload.name,
    RoleUsuario: payload.roleUsuario || payload.role || 'Admin',
    Senha: payload.senha || payload.password,
    EmailUsuario: payload.emailUsuario || payload.email,
    Chave: payload.chave || payload.key || '',
  };
}

export async function listUsersSecretary() {
  try {
    const response = await request('/UsuarioSecretaria', {
      method: 'GET',
    });
    const users = unwrapResponse(response);
    return Array.isArray(users) ? users.map(normalizeUserSecretary) : [];
  } catch (error) {
    console.error('Erro ao listar usuários da secretaria:', error);
    throw error;
  }
}

export async function getUserSecretaryById(id) {
  try {
    const response = await request(`/UsuarioSecretaria/${id}`, {
      method: 'GET',
    });
    return normalizeUserSecretary(unwrapResponse(response));
  } catch (error) {
    console.error(`Erro ao carregar usuário ${id}:`, error);
    throw error;
  }
}

export async function saveUserSecretary(payload) {
  try {
    const isUpdate = payload.id;
    const method = isUpdate ? 'PUT' : 'POST';
    const endpoint = isUpdate ? `/UsuarioSecretaria/${payload.id}` : '/UsuarioSecretaria';

    const response = await request(endpoint, {
      method,
      body: JSON.stringify(toUserSecretaryPayload(payload)),
      headers: { 'Content-Type': 'application/json' },
    });

    return normalizeUserSecretary(unwrapResponse(response));
  } catch (error) {
    console.error('Erro ao salvar usuário da secretaria:', error);
    throw error;
  }
}

export async function deleteUserSecretary(id) {
  try {
    await request(`/UsuarioSecretaria/${id}`, {
      method: 'DELETE',
    });
  } catch (error) {
    console.error(`Erro ao deletar usuário ${id}:`, error);
    throw error;
  }
}
