import { CalendarDays, Headphones, Layers3, ScanLine } from 'lucide-react';
import { AdminHeader } from '../components/navigation/AdminHeader.jsx';
import { FeatureCard } from '../components/cards/FeatureCard.jsx';
import { useLanguage } from '../hooks/useLanguage.js';
import criarEvento from '../assets/images/img-criar-evento.png';
import gerenciarEvento from '../assets/images/img-gerenciar-evento.png';
import pessoas from '../assets/images/userIconManagement.svg';
import addPessoa from '../assets/images/userIconAdd.svg';
import certificado from '../assets/images/img-certificado.png';
import { useEffect, useState } from 'react';
import { request } from '../services/apiClient.js';

export function DashboardPage() {
  const { t } = useLanguage();
  const [summary, setSummary] = useState(null);
  const [summaryError, setSummaryError] = useState('');

  useEffect(() => {
    request('/admin/dashboard/resumo')
      .then(setSummary)
      .catch((error) => setSummaryError(error.message));
  }, []);
  const cards = [
    {
      to: '/check-in',
      image: null,
      icon: ScanLine,
      title: 'Validar ingresso',
      description: 'Confirme presença por código e localização.',
    },
    {
      to: '/eventos/novo',
      image: criarEvento,
      title: t('createEvent'),
      description: t('createEventDesc'),
      className: 'feature-card--featured',
    },
    {
      to: '/eventos',
      image: gerenciarEvento,
      title: t('manageEvents'),
      description: t('manageEventsDesc'),
    },
    {
      to: '/responsaveis',
      image: pessoas,
      title: t('managePeople'),
      description: t('managePeopleDesc'),
    },
    {
      to: '/responsaveis/novo',
      image: addPessoa,
      title: t('createResponsible'),
      description: t('createResponsibleDesc'),
    },
    {
      to: '/certificados',
      image: certificado,
      title: t('manageCertificates'),
      description: t('manageCertificatesDesc'),
    },
  ];

  return (
    <>
      <AdminHeader greeting={t('adminGreeting')} backTo={null} />
      <section className="dashboard-home">
        <div className="dashboard-hero">
          <div>
            <span className="dashboard-eyebrow">{t('dashboardEyebrow')}</span>
            <h1>{t('dashboardTitle')}</h1>
            <p>{t('dashboardCopy')}</p>
          </div>

          <div className="dashboard-summary">
            <article>
              <CalendarDays size={20} />
              <strong>{summary?.eventos ?? '—'} eventos</strong>
              <span>{summary?.inscricoes ?? '—'} inscrições</span>
            </article>
            <article>
              <Layers3 size={20} />
              <strong>{summary?.taxaComparecimento ?? '—'}% de presença</strong>
              <span>{summary?.presentes ?? '—'} check-ins realizados</span>
            </article>
            <article>
              <Headphones size={20} />
              <strong>{summary?.participantesExternos ?? '—'} externos</strong>
              <span>{summary?.participantesInternos ?? '—'} internos</span>
            </article>
          </div>
          {summaryError ? <p role="alert">Não foi possível carregar os indicadores: {summaryError}</p> : null}
        </div>

        <div className="dashboard-section-title">
          <h2>{t('quickActions')}</h2>
          <p>{t('quickActionsCopy')}</p>
        </div>

        <section className="dashboard-grid">
          {cards.map((card) => (
            <FeatureCard key={card.title} {...card} />
          ))}
        </section>
      </section>
    </>
  );
}
