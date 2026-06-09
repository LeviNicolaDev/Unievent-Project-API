import * as ImagePicker from "expo-image-picker";
import { Platform } from "react-native";

function getFileExtension(uri = "", mimeType = "") {
  const fromUri = uri.split("?")[0].match(/\.([a-zA-Z0-9]+)$/)?.[1];

  if (fromUri) return fromUri.toLowerCase();

  const fromMime = mimeType.split("/")[1];

  if (fromMime) return fromMime.toLowerCase().replace("jpeg", "jpg");

  return "jpg";
}

function buildFileName(asset) {
  if (asset.fileName) return asset.fileName;

  const extension = getFileExtension(asset.uri, asset.mimeType);
  return `perfil-${Date.now()}.${extension}`;
}

function getMimeType(asset) {
  if (asset.mimeType) return asset.mimeType;

  const extension = getFileExtension(asset.uri);

  const mimeTypes = {
    heic: "image/heic",
    jpeg: "image/jpeg",
    jpg: "image/jpeg",
    png: "image/png",
    webp: "image/webp",
  };

  return mimeTypes[extension] || "image/jpeg";
}

async function pickWebPhoto() {
  if (typeof document === "undefined") {
    throw new Error("Seleção de foto indisponível neste ambiente.");
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

async function pickNativePhoto() {
  const permission = await ImagePicker.requestMediaLibraryPermissionsAsync();

  if (!permission.granted) {
    throw new Error("Permita acesso à galeria para selecionar uma foto.");
  }

  const result = await ImagePicker.launchImageLibraryAsync({
    allowsEditing: true,
    aspect: [1, 1],
    mediaTypes: ["images"],
    quality: 0.85,
  });

  if (result.canceled) {
    throw new Error("Nenhuma imagem selecionada.");
  }

  const asset = result.assets?.[0];

  if (!asset?.uri) {
    throw new Error("Não foi possível carregar a imagem selecionada.");
  }

  const name = buildFileName(asset);

  return {
    file: {
      uri: asset.uri,
      name,
      type: getMimeType(asset),
    },
    name,
    previewUri: asset.uri,
  };
}

export function pickProfilePhoto() {
  if (Platform.OS === "web") {
    return pickWebPhoto();
  }

  return pickNativePhoto();
}
