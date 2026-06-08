import { request } from './apiClient.js';

function unwrapResponse(response) {
  return response?.data || response;
}

function normalizeCertificate(certificate) {
  if (!certificate) {
    return null;
  }

  const dataCertifcado =
    certificate.dataCertifcado ||
    certificate.dataCertificado ||
    certificate.certificateDate ||
    certificate.DataCertifcado;
  const texto = certificate.texto || certificate.text || certificate.Texto || '';
  const eventoId = certificate.eventoId || certificate.eventId || certificate.EventoId || '';
  const id = certificate.id || certificate.Id;

  return {
    ...certificate,
    id,
    dataCertifcado,
    dataCertificado: dataCertifcado,
    certificateDate: dataCertifcado,
    texto,
    text: texto,
    eventoId,
    eventId: eventoId,
  };
}

function toCertificatePayload(payload) {
  return {
    DataCertifcado: payload.dataCertifcado || payload.dataCertificado || payload.certificateDate,
    Texto: payload.texto || payload.text,
    EventoId: Number(payload.eventoId || payload.eventId),
  };
}

export async function listCertificates() {
  try {
    const response = await request('/Certificado', {
      method: 'GET',
    });
    const certificates = unwrapResponse(response);
    return Array.isArray(certificates) ? certificates.map(normalizeCertificate) : [];
  } catch (error) {
    console.error('Erro ao listar certificados:', error);
    throw error;
  }
}

export async function getCertificateById(id) {
  try {
    if (!id) {
      throw new Error('Certificado inválido');
    }

    const response = await request(`/Certificado/${id}`, {
      method: 'GET',
    });
    return normalizeCertificate(unwrapResponse(response));
  } catch (error) {
    try {
      const certificates = await listCertificates();
      const certificate = certificates.find((item) => String(item.id) === String(id));

      if (certificate) {
        return certificate;
      }
    } catch {
      // Mantem o erro original do endpoint por ID.
    }

    console.error(`Erro ao carregar certificado ${id}:`, error);
    throw error;
  }
}

export async function saveCertificate(payload) {
  try {
    const isUpdate = payload.id;
    const method = isUpdate ? 'PATCH' : 'POST';
    const endpoint = isUpdate ? `/Certificado/${payload.id}` : '/Certificado';

    const response = await request(endpoint, {
      method,
      body: JSON.stringify(toCertificatePayload(payload)),
      headers: { 'Content-Type': 'application/json' },
    });

    return normalizeCertificate(unwrapResponse(response));
  } catch (error) {
    console.error('Erro ao salvar certificado:', error);
    throw error;
  }
}

export async function deleteCertificate(id) {
  try {
    await request(`/Certificado/${id}`, {
      method: 'DELETE',
    });
  } catch (error) {
    console.error(`Erro ao deletar certificado ${id}:`, error);
    throw error;
  }
}
