import * as Print from "expo-print";
import * as Sharing from "expo-sharing";
import { Platform } from "react-native";

function escapeHtml(value) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}

function formatDate(value) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "data nao informada";
  }

  return new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(date);
}

function getEventDate(event) {
  return (
    event.fullDate ||
    event.date ||
    event.apiEvent?.dataEvento ||
    event.apiEvent?.DataEvento
  );
}

function buildCertificateHtml({ certificate, event, student }) {
  const studentName = escapeHtml(student?.nome || "Aluno");
  const eventName = escapeHtml(
    certificate?.eventName || certificate?.nomeEvento || event.title
  );
  const eventDate = escapeHtml(getEventDate(event) || "data nao informada");
  const certificateText = escapeHtml(
    certificate?.text ||
      certificate?.texto ||
      `Certificamos que ${student?.nome || "Aluno"} participou do evento ${
        event.title
      }.`
  );
  const issuedAt = escapeHtml(formatDate(new Date()));
  const code = escapeHtml(
    `UNI-${String(event.id).padStart(3, "0")}-${String(
      certificate?.id || event.id
    ).padStart(3, "0")}`
  );

  return `<!doctype html>
<html lang="pt-BR">
  <head>
    <meta charset="utf-8" />
    <title>Certificado UniEvent</title>
    <style>
      @page {
        size: A4 landscape;
        margin: 0;
      }

      * {
        box-sizing: border-box;
      }

      body {
        margin: 0;
        min-height: 100vh;
        background: #f3f0ea;
        color: #171717;
        font-family: Arial, Helvetica, sans-serif;
      }

      .page {
        width: 1122px;
        height: 794px;
        padding: 52px;
        background:
          linear-gradient(135deg, rgba(255, 107, 0, 0.16), transparent 42%),
          linear-gradient(315deg, rgba(23, 23, 23, 0.10), transparent 38%),
          #fffaf3;
      }

      .frame {
        width: 100%;
        height: 100%;
        border: 4px solid #ff6b00;
        padding: 46px;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: space-between;
        text-align: center;
      }

      .brand {
        color: #ff6b00;
        font-size: 20px;
        font-weight: 900;
        letter-spacing: 0;
        text-transform: uppercase;
      }

      h1 {
        margin: 20px 0 6px;
        font-size: 54px;
        line-height: 1;
        font-weight: 900;
        letter-spacing: 0;
      }

      .subtitle {
        color: #555;
        font-size: 16px;
        font-weight: 700;
        text-transform: uppercase;
      }

      .student {
        margin: 34px 0 18px;
        color: #ff6b00;
        font-family: Georgia, "Times New Roman", serif;
        font-size: 42px;
        font-weight: 700;
      }

      .text {
        max-width: 790px;
        margin: 0 auto;
        color: #292929;
        font-size: 20px;
        line-height: 1.55;
      }

      .event {
        margin-top: 20px;
        font-size: 22px;
        font-weight: 900;
      }

      .meta {
        margin-top: 8px;
        color: #555;
        font-size: 15px;
      }

      .footer {
        width: 100%;
        display: flex;
        align-items: flex-end;
        justify-content: space-between;
        gap: 28px;
        color: #444;
        font-size: 13px;
      }

      .signature {
        min-width: 300px;
        padding-top: 12px;
        border-top: 2px solid #171717;
        font-weight: 800;
      }

      .code {
        text-align: left;
        line-height: 1.5;
      }
    </style>
  </head>
  <body>
    <main class="page">
      <section class="frame">
        <header>
          <div class="brand">UniEvent</div>
          <h1>Certificado</h1>
          <div class="subtitle">Certificado de participacao</div>
        </header>

        <section>
          <div class="student">${studentName}</div>
          <p class="text">${certificateText}</p>
          <div class="event">${eventName}</div>
          <div class="meta">Evento realizado em ${eventDate}</div>
        </section>

        <footer class="footer">
          <div class="code">
            Codigo: ${code}<br />
            Emitido em ${issuedAt}
          </div>
          <div class="signature">Organizacao UniEvent</div>
        </footer>
      </section>
    </main>
  </body>
</html>`;
}

function printCertificateOnWeb(html) {
  if (typeof window === "undefined") {
    throw new Error("Geracao de PDF indisponivel neste ambiente.");
  }

  const printWindow = window.open("", "_blank");

  if (!printWindow) {
    throw new Error("Permita pop-ups para abrir o certificado em PDF.");
  }

  printWindow.document.open();
  printWindow.document.write(html);
  printWindow.document.close();
  printWindow.focus();

  setTimeout(() => {
    printWindow.print();
  }, 300);

  return { uri: null, openedPrintDialog: true };
}

export async function generateCertificatePdf({ certificate, event, student }) {
  const html = buildCertificateHtml({ certificate, event, student });

  if (Platform.OS === "web") {
    return printCertificateOnWeb(html);
  }

  const pdf = await Print.printToFileAsync({
    html,
    width: 1122,
    height: 794,
    margins: {
      top: 0,
      right: 0,
      bottom: 0,
      left: 0,
    },
  });

  const canShare = await Sharing.isAvailableAsync();

  if (canShare) {
    await Sharing.shareAsync(pdf.uri, {
      dialogTitle: "Compartilhar certificado UniEvent",
      mimeType: "application/pdf",
      UTI: "com.adobe.pdf",
    });
  } else {
    await Print.printAsync({ uri: pdf.uri });
  }

  return { uri: pdf.uri, shared: canShare };
}
