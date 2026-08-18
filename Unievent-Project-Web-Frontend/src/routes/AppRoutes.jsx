import { Navigate, Route, Routes } from 'react-router-dom';
import { PrivateRoute } from '../components/PrivateRoute.jsx';
import { AdminLayout } from '../layouts/AdminLayout.jsx';
import { PublicLayout } from '../layouts/PublicLayout.jsx';
import { CertificateFormPage } from '../pages/CertificateFormPage.jsx';
import { CertificatesPage } from '../pages/CertificatesPage.jsx';
import { CheckInPage } from '../pages/CheckInPage.jsx';
import { ConfirmEmailPage } from '../pages/ConfirmEmailPage.jsx';
import { DashboardPage } from '../pages/DashboardPage.jsx';
import { EventFormPage } from '../pages/EventFormPage.jsx';
import { EventPreviewPage } from '../pages/EventPreviewPage.jsx';
import { EventsPage } from '../pages/EventsPage.jsx';
import { LandingPage } from '../pages/LandingPage.jsx';
import { LoginPage } from '../pages/LoginPage.jsx';
import { NotFoundPage } from '../pages/NotFoundPage.jsx';
import { PeopleManagementPage } from '../pages/PeopleManagementPage.jsx';
import { ResponsibleFormPage } from '../pages/ResponsibleFormPage.jsx';
import { SupportPage } from '../pages/SupportPage.jsx';

export function AppRoutes() {
  return (
    <Routes>
      <Route element={<PublicLayout />}>
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/confirmar-email" element={<ConfirmEmailPage />} />
      </Route>

      <Route element={<AdminLayout />}>
        <Route path="/home" element={<PrivateRoute><DashboardPage /></PrivateRoute>} />
        <Route path="/eventos" element={<PrivateRoute><EventsPage /></PrivateRoute>} />
        <Route path="/eventos/novo" element={<PrivateRoute><EventFormPage mode="create" /></PrivateRoute>} />
        <Route path="/eventos/:id/editar" element={<PrivateRoute><EventFormPage mode="edit" /></PrivateRoute>} />
        <Route path="/eventos/preview" element={<PrivateRoute><EventPreviewPage /></PrivateRoute>} />
        <Route path="/responsaveis" element={<PrivateRoute><PeopleManagementPage /></PrivateRoute>} />
        <Route path="/responsaveis/novo" element={<PrivateRoute><ResponsibleFormPage /></PrivateRoute>} />
        <Route path="/responsaveis/:id/editar" element={<PrivateRoute><ResponsibleFormPage mode="edit" /></PrivateRoute>} />
        <Route path="/certificados" element={<PrivateRoute><CertificatesPage /></PrivateRoute>} />
        <Route path="/certificados/novo" element={<PrivateRoute><CertificateFormPage /></PrivateRoute>} />
        <Route path="/certificados/:id/editar" element={<PrivateRoute><CertificateFormPage mode="edit" /></PrivateRoute>} />
        <Route path="/check-in" element={<PrivateRoute><CheckInPage /></PrivateRoute>} />
        <Route path="/suporte" element={<PrivateRoute><SupportPage /></PrivateRoute>} />
      </Route>

      <Route path="/index.php" element={<Navigate to="/" replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
