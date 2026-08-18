import { useState } from 'react';
import { AdminHeader } from '../components/navigation/AdminHeader.jsx';
import { Button } from '../components/ui/Button.jsx';
import { getCurrentPosition, validateCheckIn } from '../services/checkInService.js';

export function CheckInPage() {
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState('');

  async function handleSubmit(event) {
    event.preventDefault();
    if (!code.trim()) return;
    setLoading(true);
    setError('');
    setResult(null);
    try {
      const position = await getCurrentPosition();
      setResult(await validateCheckIn(code, position));
      setCode('');
    } catch (checkInError) {
      setError(checkInError.message || 'Não foi possível validar o ingresso.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      <AdminHeader greeting="Check-in de participantes" backTo="/home" />
      <section className="dashboard-home">
        <div className="dashboard-section-title">
          <h1>Validar ingresso</h1>
          <p>Informe o código exibido no QR. A localização deste dispositivo será validada pelo servidor.</p>
        </div>
        <form className="event-form" onSubmit={handleSubmit}>
          <label>
            Código do ingresso
            <input
              value={code}
              onChange={(event) => setCode(event.target.value)}
              autoComplete="off"
              placeholder="Cole ou leia o código do QR"
              required
            />
          </label>
          <Button type="submit" disabled={loading}>{loading ? 'Validando...' : 'Confirmar check-in'}</Button>
        </form>
        {error ? <p role="alert">{error}</p> : null}
        {result ? <p role="status">Presença confirmada para {result.nomeAluno || 'participante'}.</p> : null}
      </section>
    </>
  );
}
