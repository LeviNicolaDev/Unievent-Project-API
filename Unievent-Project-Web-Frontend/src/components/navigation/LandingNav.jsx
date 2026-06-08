import { Github, Moon, Sun } from 'lucide-react';
import { Link } from 'react-router-dom';
import logo from '../../assets/images/logo.svg';
import { useLanguage } from '../../hooks/useLanguage.js';

export function LandingNav({ isLight, onThemeToggle }) {
  const { language, setLanguage, t } = useLanguage();

  return (
    <nav className="landing-modern-nav">
      <a className="landing-modern-brand" href="#inicio" aria-label="UniEvent">
        <img src={logo} alt="" />
      </a>

      <div className="landing-modern-links">
        <a href="#sobre">{t('landingAbout')}</a>
        <a href="#aplicativo">{t('landingApp')}</a>
        <a href="#integrantes">{t('landingMembers')}</a>
        <a href="#contato">{t('landingContact')}</a>
      </div>

      <div className="landing-modern-actions">
        <button
          className="landing-language-toggle"
          type="button"
          onClick={() => setLanguage(language === 'en' ? 'pt' : 'en')}
          aria-label={t('language')}
          title={t('language')}
        >
          {language === 'en' ? 'PT' : 'EN'}
        </button>
        <button
          className="landing-theme-toggle"
          type="button"
          onClick={onThemeToggle}
          aria-label={isLight ? t('activateDark') : t('activateLight')}
          title={isLight ? t('themeDark') : t('themeLight')}
        >
          {isLight ? <Moon size={19} /> : <Sun size={19} />}
        </button>
        <a
          className="landing-icon-action"
          href="https://github.com/0RyanSouza0/landing-page-unievent"
          target="_blank"
          rel="noreferrer"
          aria-label="GitHub"
          title="GitHub"
        >
          <Github size={20} />
        </a>
        <Link className="landing-login-action" to="/login">{t('login')}</Link>
      </div>
    </nav>
  );
}
