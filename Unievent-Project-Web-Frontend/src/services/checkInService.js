import { request } from './apiClient.js';

export function validateCheckIn(codigoIngresso, position) {
  return request('/Evento/check-in', {
    method: 'POST',
    body: JSON.stringify({
      codigoIngresso: codigoIngresso.trim(),
      latitude: position.coords.latitude,
      longitude: position.coords.longitude,
      precisaoMetros: position.coords.accuracy,
    }),
  });
}

export function getCurrentPosition() {
  return new Promise((resolve, reject) => {
    if (!navigator.geolocation) {
      reject(new Error('Geolocalização não disponível neste navegador.'));
      return;
    }
    navigator.geolocation.getCurrentPosition(resolve, reject, {
      enableHighAccuracy: true,
      timeout: 15000,
      maximumAge: 0,
    });
  });
}
