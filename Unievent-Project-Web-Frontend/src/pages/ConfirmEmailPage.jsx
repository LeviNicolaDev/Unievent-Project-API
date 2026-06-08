import { AlertCircle, CheckCircle2 } from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";
import { useLanguage } from "../hooks/useLanguage.js";

export function ConfirmEmailPage() {
  const [searchParams] = useSearchParams();
  const { t } = useLanguage();
  const confirmationKey = searchParams.get("chave");

  return (
    <main className="auth-page">
      <section className="auth-shell">
        <section className="auth-card" style={{ maxWidth: 520, margin: "0 auto" }}>
          <div className="auth-card-header">
            <div className="auth-card-icon">
              {confirmationKey ? <CheckCircle2 size={24} /> : <AlertCircle size={24} />}
            </div>
            <div>
              <span>UniEvent Admin</span>
              <h2>{t("confirmEmailTitle")}</h2>
            </div>
          </div>

          <p style={{ color: "var(--auth-muted)", lineHeight: 1.6 }}>
            {confirmationKey ? t("confirmEmailPendingApi") : t("confirmEmailMissingKey")}
          </p>

          <Link className="auth-submit" to="/login" style={{ textAlign: "center" }}>
            {t("confirmEmailLoginButton")}
          </Link>
        </section>
      </section>
    </main>
  );
}
