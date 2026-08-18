import { useState } from "react";
import { ScrollView, Text, TouchableOpacity, View } from "react-native";
import AuthInput from "../components/AuthInput";
import Logo from "../components/Logo";
import Screen from "../components/Screen";
import { BLACK, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { styles } from "../styles/globalStyles";
import { pickProfilePhoto } from "../utils/photoPicker";

export default function SignUpScreen({ theme, navigation, toggleTheme }) {
  const isLight = theme.mode === "light";
  const { signIn, signUp } = useAuth();
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [dataNascimento, setDataNascimento] = useState("");
  const [senha, setSenha] = useState("");
  const [confirmacaoSenha, setConfirmacaoSenha] = useState("");
  const [fotoPerfil, setFotoPerfil] = useState(null);
  const [fotoNome, setFotoNome] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [tipoParticipante, setTipoParticipante] = useState("Externo");
  const [instituicaoId, setInstituicaoId] = useState("");

  function normalizeBirthDate(value) {
    const trimmed = value.trim();
    const digitsDate = trimmed.match(/^(\d{2})(\d{2})(\d{4})$/);
    const brDate = trimmed.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);

    if (digitsDate) {
      return `${digitsDate[3]}-${digitsDate[2]}-${digitsDate[1]}`;
    }

    if (brDate) {
      return `${brDate[3]}-${brDate[2]}-${brDate[1]}`;
    }

    return trimmed;
  }

  async function handleSignUp() {
    if (
      !nome.trim() ||
      !email.trim() ||
      !dataNascimento.trim() ||
      !senha ||
      !fotoPerfil ||
      (tipoParticipante === "Interno" && !instituicaoId.trim())
    ) {
      setError("Preencha nome, e-mail, data, senha e foto de perfil.");
      return;
    }

    if (senha !== confirmacaoSenha) {
      setError("A confirmação de senha não confere.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      const birthDate = normalizeBirthDate(dataNascimento);
      await signUp({
        nome,
        email,
        senha,
        dataNascimento: birthDate,
        fotoPerfil,
        tipoParticipante,
        instituicaoId,
      });
      await signIn({ email, senha });
      navigation.replace("Home");
    } catch (signUpError) {
      setError(signUpError.message || "Não foi possível criar sua conta.");
    } finally {
      setLoading(false);
    }
  }

  async function handlePickPhoto() {
    setError("");

    try {
      const photo = await pickProfilePhoto();
      setFotoPerfil(photo.file);
      setFotoNome(photo.name);
    } catch (photoError) {
      setError(photoError.message);
    }
  }

  return (
    <Screen theme={theme} bg={isLight ? BLACK : ORANGE} pad={false}>
      <View
        style={[
          styles.authScreen,
          { backgroundColor: isLight ? BLACK : ORANGE },
        ]}
      >
        <View
          style={[
            styles.authTopSignup,
            { backgroundColor: isLight ? BLACK : ORANGE },
          ]}
        >
          <Logo small color="#fff" />
        </View>

        <ScrollView
          showsVerticalScrollIndicator={false}
          style={[
            styles.authBottomSignup,
            { backgroundColor: isLight ? BLACK : ORANGE },
          ]}
          contentContainerStyle={styles.authBottomSignupContent}
        >
          <View style={styles.row}>
            <Text style={styles.authTitleWhite}>Cadastro</Text>
          </View>

          <Text style={styles.authLabel}>Nome</Text>
          <AuthInput
            icon="person"
            autoCapitalize="words"
            onChangeText={setNome}
            placeholder="Insira aqui seu nome"
            theme={signupInputTheme}
            value={nome}
          />

          <Text style={styles.authLabel}>E-mail</Text>
          <AuthInput
            icon="mail"
            keyboardType="email-address"
            onChangeText={setEmail}
            placeholder="email@fatec.sp.gov.br"
            theme={signupInputTheme}
            value={email}
          />

          <Text style={styles.authLabel}>Tipo de participante</Text>
          <View style={styles.row}>
            {['Externo', 'Interno'].map((tipo) => (
              <TouchableOpacity
                key={tipo}
                onPress={() => setTipoParticipante(tipo)}
                style={[styles.authPhotoButton, { opacity: tipoParticipante === tipo ? 1 : 0.55, flex: 1 }]}
              >
                <Text style={styles.authPhotoButtonText}>{tipo}</Text>
              </TouchableOpacity>
            ))}
          </View>

          {tipoParticipante === "Interno" ? <>
            <Text style={styles.authLabel}>Código da instituição</Text>
            <AuthInput
              icon="business"
              keyboardType="numeric"
              onChangeText={setInstituicaoId}
              placeholder="ID da instituição"
              theme={signupInputTheme}
              value={instituicaoId}
            />
          </> : null}

          <Text style={styles.authLabel}>Data de nascimento</Text>
          <AuthInput
            icon="calendar"
            keyboardType="numeric"
            onChangeText={setDataNascimento}
            placeholder="dd/mm/aaaa"
            theme={signupInputTheme}
            value={dataNascimento}
          />

          <Text style={styles.authLabel}>Foto de perfil</Text>
          <TouchableOpacity
            activeOpacity={0.85}
            style={styles.authPhotoButton}
            onPress={handlePickPhoto}
          >
            <Text style={styles.authPhotoButtonText}>
              {fotoNome || "Selecionar foto"}
            </Text>
          </TouchableOpacity>

          <Text style={styles.authLabel}>Senha</Text>
          <AuthInput
            icon="lock-closed"
            onChangeText={setSenha}
            placeholder="Insira aqui sua senha"
            secure
            theme={signupInputTheme}
            value={senha}
          />

          <Text style={styles.authLabel}>Confirmação de Senha</Text>
          <AuthInput
            icon="lock-closed"
            onChangeText={setConfirmacaoSenha}
            placeholder="Insira aqui sua senha"
            secure
            theme={signupInputTheme}
            value={confirmacaoSenha}
          />

          <TouchableOpacity
            disabled={loading}
            style={[
              styles.signUpButton,
              {
                backgroundColor: BLACK,
                borderWidth: isLight ? 1 : 0,
                borderColor: isLight ? "#FFFFFF" : "transparent",
                opacity: loading ? 0.7 : 1,
              },
            ]}
            onPress={handleSignUp}
          >
            <Text style={styles.signUpButtonText}>
              {loading ? "Criando..." : "Criar Conta"}
            </Text>
          </TouchableOpacity>

          {error ? <Text style={styles.authErrorText}>{error}</Text> : null}

          <TouchableOpacity style={styles.termsRow}>
            <View style={styles.termsBox} />
            <Text style={styles.authMiniWhite}>
              Concordo com os Termos de Uso e Privacidade
            </Text>
          </TouchableOpacity>
        </ScrollView>
      </View>
    </Screen>
  );
}

const signupInputTheme = {
  text: "#FFFFFF",
  muted: "#FFFFFF",
  border: "#FFFFFF",
};
