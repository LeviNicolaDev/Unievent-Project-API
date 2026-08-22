import { useState } from 'react';
import { AdminHeader } from '../components/navigation/AdminHeader.jsx';
import { Button } from '../components/ui/Button.jsx';
import { useAuth } from '../contexts/AuthContext.jsx';
import { validateCheckIn } from '../services/checkInService.js';

export function CheckInPage() {
  const { user } = useAuth();
  const [code, setCode] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState('');
  const backTo = user?.roleUsuario === 'Secretaria' ? '/instituicao/dashboard' : '/home';

  async function handleSubmit(event) {
    event.preventDefault();
    if (!code.trim()) return;
    setLoading(true);
    setError('');
    setResult(null);
    try {
      setResult(await validateCheckIn(code));
      setCode('');
    } catch (checkInError) {
      setError(checkInError.message || 'Não foi possível validar o ingresso.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      <AdminHeader greeting="Check-in de participantes" backTo={backTo} />
      <section className="dashboard-home">
        <div className="dashboard-section-title">
          <h1>Validar ingresso</h1>
          <p>Informe ou leia o código exibido no QR Code do aluno para confirmar a presença.</p>
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
