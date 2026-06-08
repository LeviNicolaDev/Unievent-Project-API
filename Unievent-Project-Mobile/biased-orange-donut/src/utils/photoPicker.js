import { Platform } from "react-native";

export function pickProfilePhoto() {
  if (Platform.OS !== "web" || typeof document === "undefined") {
    return Promise.reject(
      new Error("Seleção de foto disponível somente no web nesta versão.")
    );
  }

  return new Promise((resolve, reject) => {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = "image/*";
    input.onchange = () => {
      const file = input.files?.[0];

      if (!file) {
        reject(new Error("Nenhuma imagem selecionada."));
        return;
      }

      resolve({
        file,
        name: file.name,
        previewUri: URL.createObjectURL(file),
      });
    };
    input.click();
  });
}
