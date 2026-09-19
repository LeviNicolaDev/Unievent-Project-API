import { Ionicons } from "@expo/vector-icons";
import { useEffect, useState } from "react";
import {
  Image,
  Modal,
  ScrollView,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from "react-native";

import BottomNav from "../components/BottomNav";
import Screen from "../components/Screen";
import ThemeButton from "../components/ThemeButton";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";
import {
  formatDateForInput,
  normalizeBrazilianDateInput,
} from "../utils/dateFormat";
import { pickProfilePhoto } from "../utils/photoPicker";

export default function ProfileScreen({
  theme,
  route,
  navigation,
  toggleTheme,
}) {
  const isLight = theme.mode === "light";
  const [modalVisible, setModalVisible] = useState(false);
  const { student } = useAuth();
  const { events, favoriteEvents, registeredEvents } = useEvents();
  const birthDateLabel = formatDateForInput(student?.dataNascimento);

  const tags = [
    "Palestras",
    "Workshop",
    "E-Sportes",
    "Música",
    "Cursos",
    "Oficina",
    "Hackathon",
    "Feiras",
  ];

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.profileScroll}
      >
        <View style={styles.profileHeader}>
          <Text style={[styles.profileHeaderTitle, { color: theme.text }]}>
            Perfil
          </Text>

          <View style={styles.profileHeaderIcons}>
            <TouchableOpacity onPress={() => navigation.navigate("Settings")}>
              <Ionicons
                name="settings-outline"
                color={isLight ? BLACK : "#FFFFFF"}
                size={18}
              />
            </TouchableOpacity>
            <ThemeButton theme={theme} toggleTheme={toggleTheme} />
          </View>
        </View>

        <View style={styles.profileMain}>
          {student?.fotoPerfil ? (
            <Image
              source={{ uri: student.fotoPerfil }}
              style={styles.avatarImage}
            />
          ) : (
            <View style={styles.avatarCircle}>
              <Text style={styles.avatarEmoji}>👨🏻‍💻</Text>
            </View>
          )}

          <Text style={[styles.profileName, { color: theme.text }]}>
            {student?.nome || "Aluno"}
          </Text>

          <Text style={[styles.profileEmail, { color: theme.soft }]}>
            {student?.email || "aluno@fatec.sp.gov.br"}
          </Text>

          {birthDateLabel ? (
            <Text style={[styles.profileEmail, { color: theme.soft }]}>
              {birthDateLabel}
            </Text>
          ) : null}

          {student?.instituicaoNome ? (
            <Text style={[styles.profileEmail, { color: theme.soft }]}>
              {student.instituicaoNome}
            </Text>
          ) : null}

          <View style={styles.profileBtns}>
            <TouchableOpacity
              onPress={() => setModalVisible(true)}
              style={[
                styles.profileButton,
                { backgroundColor: isLight ? BLACK : ORANGE },
              ]}
            >
              <Text style={styles.profileButtonText}>Editar perfil</Text>
            </TouchableOpacity>

            <TouchableOpacity
              onPress={() => navigation.navigate("Favorites")}
              style={[
                styles.profileButton,
                { backgroundColor: isLight ? BLACK : ORANGE },
              ]}
            >
              <Text style={styles.profileButtonText}>Eventos favoritos</Text>
            </TouchableOpacity>

            <TouchableOpacity
              onPress={() => navigation.navigate("MyEvents")}
              style={[
                styles.profileButton,
                { backgroundColor: isLight ? BLACK : ORANGE },
              ]}
            >
              <Text style={styles.profileButtonText}>Meus eventos</Text>
            </TouchableOpacity>
          </View>
        </View>

        <View style={styles.profileStatsRow}>
          <StatBox
            title={String(events.length).padStart(2, "0")}
            label="Eventos"
            theme={theme}
          />
          <StatBox
            title={String(registeredEvents.length).padStart(2, "0")}
            label="Ingressos"
            theme={theme}
          />
          <StatBox
            title={String(favoriteEvents.length).padStart(2, "0")}
            label="Favoritos"
            theme={theme}
          />
        </View>

        <Text style={[styles.profileSectionTitle, { color: theme.text }]}>
          Gostos
        </Text>

        <View style={styles.profileTags}>
          {tags.map((tag) => (
            <View
              key={tag}
              style={[
                styles.profileTag,
                {
                  borderColor: isLight ? BLACK : ORANGE,
                  backgroundColor: isLight ? "transparent" : "#1A1A1A",
                },
              ]}
            >
              <Text
                style={[
                  styles.profileTagText,
                  { color: isLight ? BLACK : "#FFFFFF" },
                ]}
              >
                {tag}
              </Text>
            </View>
          ))}
        </View>
      </ScrollView>

      <EditProfileModal
        visible={modalVisible}
        onClose={() => setModalVisible(false)}
        student={student}
        theme={theme}
      />

      <BottomNav routeName={route.name} theme={theme} navigation={navigation} />
    </Screen>
  );
}

function EditProfileModal({ visible, onClose, student, theme }) {
  const isLight = theme.mode === "light";
  const { updateProfile } = useAuth();
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [dataNascimento, setDataNascimento] = useState("");
  const [senha, setSenha] = useState("");
  const [confirmacaoSenha, setConfirmacaoSenha] = useState("");
  const [fotoPerfil, setFotoPerfil] = useState(null);
  const [fotoPreview, setFotoPreview] = useState("");
  const [fotoNome, setFotoNome] = useState("");
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!visible) return;

    setNome(student?.nome || "");
    setEmail(student?.email || "");
    setDataNascimento(formatDateForInput(student?.dataNascimento));
    setSenha("");
    setConfirmacaoSenha("");
    setFotoPerfil(null);
    setFotoPreview(student?.fotoPerfil || "");
    setFotoNome("");
    setError("");
  }, [student, visible]);

  async function handlePickPhoto() {
    setError("");

    try {
      const photo = await pickProfilePhoto();
      setFotoPerfil(photo.file);
      setFotoPreview(photo.previewUri);
      setFotoNome(photo.name);
    } catch (photoError) {
      setError(photoError.message);
    }
  }

  async function handleSave() {
    if (!nome.trim()) {
      setError("Informe seu nome.");
      return;
    }

    if (senha && senha !== confirmacaoSenha) {
      setError("A confirmação de senha não confere.");
      return;
    }

    setSaving(true);
    setError("");

    try {
      await updateProfile({
        nome,
        senha,
        dataNascimento: dataNascimento
          ? normalizeBrazilianDateInput(dataNascimento)
          : undefined,
        fotoPerfil,
      });
      onClose();
    } catch (saveError) {
      setError(saveError.message || "Não foi possível salvar as alterações.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Modal visible={visible} transparent animationType="fade">
      <View style={styles.modalOverlay}>
        <View
          style={[
            styles.editModal,
            { backgroundColor: isLight ? "#FFFFFF" : BLACK },
          ]}
        >
          <View style={styles.modalHeader}>
            <Text style={[styles.modalTitle, { color: theme.text }]}>
              Editar perfil
            </Text>

            <TouchableOpacity onPress={onClose}>
              <Ionicons
                name="close"
                size={24}
                color={isLight ? BLACK : "#FFFFFF"}
              />
            </TouchableOpacity>
          </View>

          <ScrollView showsVerticalScrollIndicator={false}>
            <View style={styles.profilePhotoEditRow}>
              {fotoPreview ? (
                <Image
                  source={{ uri: fotoPreview }}
                  style={styles.profilePhotoPreview}
                />
              ) : (
                <View style={styles.profilePhotoPreviewFallback}>
                  <Ionicons
                    name="person"
                    size={28}
                    color={isLight ? BLACK : "#FFFFFF"}
                  />
                </View>
              )}

              <TouchableOpacity
                activeOpacity={0.85}
                style={[
                  styles.profilePhotoButton,
                  { borderColor: isLight ? BLACK : ORANGE },
                ]}
                onPress={handlePickPhoto}
              >
                <Ionicons
                  name="camera-outline"
                  size={17}
                  color={isLight ? BLACK : "#FFFFFF"}
                />
                <Text
                  numberOfLines={1}
                  style={[
                    styles.profilePhotoButtonText,
                    { color: isLight ? BLACK : "#FFFFFF" },
                  ]}
                >
                  {fotoNome || "Alterar foto"}
                </Text>
              </TouchableOpacity>
            </View>

            <ProfileInput
              label="Nome"
              icon="person"
              autoCapitalize="words"
              onChangeText={setNome}
              placeholder="Insira aqui seu nome"
              theme={theme}
              value={nome}
            />

            <ProfileInput
              label="Data de nascimento"
              icon="calendar"
              keyboardType="numeric"
              onChangeText={setDataNascimento}
              placeholder="dd/mm/aaaa"
              theme={theme}
              value={dataNascimento}
            />

            <ProfileInput
              label="Senha"
              icon="lock-closed"
              onChangeText={setSenha}
              placeholder="Insira aqui sua senha"
              secure
              theme={theme}
              value={senha}
            />

            <ProfileInput
              label="Confirmação de senha"
              icon="lock-closed"
              onChangeText={setConfirmacaoSenha}
              placeholder="Confirme sua senha"
              secure
              theme={theme}
              value={confirmacaoSenha}
            />

            {error ? (
              <Text style={[styles.profileModalError, { color: theme.text }]}>
                {error}
              </Text>
            ) : null}

            <TouchableOpacity
              disabled={saving}
              style={[
                styles.modalSaveButton,
                {
                  backgroundColor: isLight ? BLACK : ORANGE,
                  opacity: saving ? 0.7 : 1,
                },
              ]}
              onPress={handleSave}
            >
              <Text style={styles.modalSaveButtonText}>
                {saving ? "Salvando..." : "Salvar alterações"}
              </Text>
            </TouchableOpacity>
          </ScrollView>
        </View>
      </View>
    </Modal>
  );
}

function ProfileInput({
  autoCapitalize = "none",
  editable = true,
  icon,
  keyboardType = "default",
  label,
  onChangeText,
  placeholder,
  secure,
  theme,
  value,
}) {
  const isLight = theme.mode === "light";
  const [passwordVisible, setPasswordVisible] = useState(false);
  const shouldHideText = Boolean(secure && !passwordVisible);

  return (
    <View style={styles.profileInputGroup}>
      <Text style={[styles.profileInputLabel, { color: theme.text }]}>
        {label}
      </Text>

      <View
        style={[
          styles.profileInputBox,
          { borderColor: isLight ? BLACK : ORANGE },
        ]}
      >
        <Ionicons name={icon} size={17} color={isLight ? BLACK : "#FFFFFF"} />

        <TextInput
          autoCapitalize={autoCapitalize}
          editable={editable}
          keyboardType={keyboardType}
          onChangeText={onChangeText}
          placeholder={placeholder}
          placeholderTextColor={isLight ? "#777777" : "#DADADA"}
          secureTextEntry={shouldHideText}
          style={[styles.profileInput, { color: isLight ? BLACK : "#FFFFFF" }]}
          value={value}
        />

        {secure && (
          <TouchableOpacity
            accessibilityLabel={
              passwordVisible ? "Ocultar senha" : "Visualizar senha"
            }
            accessibilityRole="button"
            hitSlop={10}
            onPress={() => setPasswordVisible((visible) => !visible)}
          >
            <Ionicons
              name={passwordVisible ? "eye-outline" : "eye-off-outline"}
              size={18}
              color={isLight ? BLACK : "#FFFFFF"}
            />
          </TouchableOpacity>
        )}
      </View>
    </View>
  );
}

function StatBox({ title, label, theme }) {
  const isLight = theme.mode === "light";

  return (
    <View
      style={[
        styles.profileStatBox,
        { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
      ]}
    >
      <Text
        style={[styles.profileStatNumber, { color: isLight ? BLACK : ORANGE }]}
      >
        {title}
      </Text>

      <Text style={[styles.profileStatLabel, { color: theme.soft }]}>
        {label}
      </Text>
    </View>
  );
}
