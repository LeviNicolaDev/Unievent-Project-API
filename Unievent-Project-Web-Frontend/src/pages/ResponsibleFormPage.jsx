import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Camera, ShieldCheck, UserPlus, UsersRound } from 'lucide-react';
import { AdminHeader } from '../components/navigation/AdminHeader.jsx';
import { Button } from '../components/ui/Button.jsx';
import { FormField } from '../components/forms/FormField.jsx';
import { Modal } from '../components/ui/Modal.jsx';
import { useLanguage } from '../hooks/useLanguage.js';
import { getResponsibleById, saveResponsible } from '../services/responsibleService.js';
import { getAssetUrl } from '../utils/formatters.js';

export function ResponsibleFormPage({ mode = 'create' }) {
  const { t } = useLanguage();
  const { id } = useParams();
  const [name, setName] = useState('');
  const [preview, setPreview] = useState(getAssetUrl('avatar-placeholder.png'));
  const [photo, setPhoto] = useState(null);
  const [open, setOpen] = useState(false);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(mode === 'edit');
  const navigate = useNavigate();
  const isEdit = mode === 'edit';

  useEffect(() => {
    let isMounted = true;

    async function loadResponsible() {
      if (!isEdit) return;

      setIsLoading(true);
      setError('');

      try {
        const responsible = await getResponsibleById(id);
        if (!isMounted) return;

        setName(responsible?.nome || responsible?.name || '');
        setPreview(responsible?.fotoPerfil || responsible?.avatar || getAssetUrl('avatar-placeholder.png'));
      } catch (err) {
        if (isMounted) {
          setError(err.message || 'Erro ao carregar responsável');
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    loadResponsible();

    return () => {
      isMounted = false;
    };
  }, [id, isEdit]);

  async function submit(event) {
    event.preventDefault();
    setError('');

    try {
      await saveResponsible({ id: isEdit ? id : undefined, nome: name, fotoPerfil: photo });
      setOpen(true);
    } catch (err) {
      setError(err.message || 'Erro ao salvar responsável');
    }
  }

  function handleImage(event) {
    const file = event.target.files?.[0];
    if (file) {
      setPhoto(file);
      setPreview(URL.createObjectURL(file));
    }
  }

  if (isLoading) {
    return (
      <>
        <AdminHeader title={isEdit ? t('editResponsible') : t('createResponsible')} backTo="/responsaveis" />
        <main className="responsible-editor-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '60vh' }}>
          <p>{t('loading') || 'Carregando...'}</p>
        </main>
      </>
    );
  }

  return (
    <>
      <AdminHeader title={isEdit ? t('editResponsible') : t('createResponsible')} backTo="/responsaveis" />
      <main className="responsible-editor-page">
        <section className="responsible-editor-hero">
          <div>
            <span className="responsible-editor-eyebrow">{t('responsibleNewEyebrow')}</span>
            <h1>{isEdit ? t('editResponsible') : t('responsibleNewTitle')}</h1>
            <p>{t('responsibleNewCopy')}</p>
          </div>

          <div className="responsible-editor-notes" aria-label={t('formSummary')}>
            <article>
              <UserPlus size={20} />
              <span>{t('accessProfile')}</span>
            </article>
            <article>
              <UsersRound size={20} />
              <span>{t('organizerTeam')}</span>
            </article>
            <article>
              <ShieldCheck size={20} />
              <span>{t('responsibleForEvents')}</span>
            </article>
          </div>
        </section>

        <section className="responsible-editor-card">
          <div className="responsible-editor-card-header">
            <div>
              <span>{t('form')}</span>
              <h2>{t('responsibleData')}</h2>
            </div>
            <p>{t('responsibleHint')}</p>
          </div>

          <form className="responsible-form" onSubmit={submit}>
            {error && (
              <div className="auth-error-message" style={{ color: '#dc2626', marginBottom: '1rem' }}>
                {error}
              </div>
            )}

            <FormField label={t('responsibleName')}>
              <input placeholder={t('responsibleName')} value={name} onChange={(event) => setName(event.target.value)} required />
            </FormField>

            <FormField label={t('profilePhoto')}>
              <span className="profile-upload-box">
                <input type="file" accept="image/*" onChange={handleImage} required={!isEdit} />
                <img
                  src={preview}
                  alt={t('responsiblePreviewAlt')}
                  onError={(event) => {
                    event.currentTarget.src = getAssetUrl('avatar-placeholder.png');
                  }}
                />
                <span><Camera size={18} />{t('changePhoto')}</span>
              </span>
            </FormField>

            <div className="responsible-form-actions">
              <Button type="submit">{t('send')}</Button>
            </div>
          </form>
        </section>
      </main>
      <Modal
        open={open}
        title={isEdit ? t('editResponsible') : t('createResponsible')}
        message={isEdit ? 'Responsável atualizado com sucesso.' : t('responsibleCreated')}
        image={getAssetUrl('emoteAcess.png')}
        confirmText={t('ok')}
        onClose={() => navigate('/responsaveis')}
        onConfirm={() => navigate('/responsaveis')}
      />
    </>
  );
}
