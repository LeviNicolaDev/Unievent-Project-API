import { CalendarDays, Headphones, Layers3 } from 'lucide-react';
import { AdminHeader } from '../components/navigation/AdminHeader.jsx';
import { FeatureCard } from '../components/cards/FeatureCard.jsx';
import { useLanguage } from '../hooks/useLanguage.js';
import criarEvento from '../assets/images/img-criar-evento.png';
import gerenciarEvento from '../assets/images/img-gerenciar-evento.png';
import pessoas from '../assets/images/userIconManagement.svg';
import addPessoa from '../assets/images/userIconAdd.svg';
import certificado from '../assets/images/img-certificado.png';

export function DashboardPage() {
  const { t } = useLanguage();
  const cards = [
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
              <strong>{t('events')}</strong>
              <span>{t('dashboardEventsDesc')}</span>
            </article>
            <article>
              <Layers3 size={20} />
              <strong>{t('dashboardModules')}</strong>
              <span>{t('dashboardModulesDesc')}</span>
            </article>
            <article>
              <Headphones size={20} />
              <strong>{t('navSupport')}</strong>
              <span>{t('dashboardSupportDesc')}</span>
            </article>
          </div>
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
