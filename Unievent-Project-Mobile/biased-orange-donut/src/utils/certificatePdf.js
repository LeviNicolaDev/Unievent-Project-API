import * as Print from "expo-print";
import * as Sharing from "expo-sharing";
import { File, Paths } from "expo-file-system";
import { Platform } from "react-native";
import { API_URL } from "../services/api";

// O certificado oficial é o PDF persistido pelo backend após a validação da Secretaria.
export async function generateCertificatePdf({ event, token }) {
  if (!token) throw new Error("Faça login para baixar seu certificado.");
  const url = `${API_URL}/Certificado/eventos/${encodeURIComponent(event.id)}/pdf`;
  const response = await fetch(url, { headers: { Authorization: `Bearer ${token}` } });
  if (!response.ok) {
    throw new Error(response.status === 401 ? "Sessão expirada. Faça login novamente."
      : "O certificado ainda não está disponível. Atualize sua presença e tente novamente.");
  }
  if (!response.headers.get("content-type")?.includes("application/pdf"))
    throw new Error("Não foi possível obter o PDF do certificado.");

  if (Platform.OS === "web") {
    const objectUrl = URL.createObjectURL(await response.blob());
    const link = document.createElement("a");
    link.href = objectUrl;
    link.download = `certificado-evento-${event.id}.pdf`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    setTimeout(() => URL.revokeObjectURL(objectUrl), 1000);
    return;
  }

  const file = new File(Paths.cache, `certificado-${Date.now()}.pdf`);
  try {
    file.write(new Uint8Array(await response.arrayBuffer()));
    if (await Sharing.isAvailableAsync()) {
      await Sharing.shareAsync(file.uri, {
        dialogTitle: "Compartilhar certificado UniEvent", mimeType: "application/pdf", UTI: "com.adobe.pdf",
      });
    } else {
      await Print.printAsync({ uri: file.uri });
    }
  } finally {
    if (file.exists) file.delete();
  }
}
