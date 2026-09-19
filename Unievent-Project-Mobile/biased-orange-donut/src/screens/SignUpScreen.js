import { useEffect, useMemo, useState } from "react";
import { ScrollView, Text, TouchableOpacity, View } from "react-native";
import AuthHeader from "../components/AuthHeader";
import AuthInput from "../components/AuthInput";
import Logo from "../components/Logo";
import Screen from "../components/Screen";
import SelectField from "../components/SelectField";
import ThemeButton from "../components/ThemeButton";
import { BLACK, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { institutionsApi } from "../services/api";
import { styles } from "../styles/globalStyles";
import { normalizeBrazilianDateInput } from "../utils/dateFormat";
import { pickProfilePhoto } from "../utils/photoPicker";

export default function SignUpScreen({ theme, navigation, toggleTheme }) {
  const isLight = theme.mode === "light";
  const { signUp } = useAuth();
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [dataNascimento, setDataNascimento] = useState("");
  const [senha, setSenha] = useState("");
  const [confirmacaoSenha, setConfirmacaoSenha] = useState("");
  const [fotoPerfil, setFotoPerfil] = useState(null);
  const [fotoNome, setFotoNome] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);
  const [instituicaoId, setInstituicaoId] = useState("");
  const [institutions, setInstitutions] = useState([]);
  const [institutionsLoading, setInstitutionsLoading] = useState(true);
  const [institutionsError, setInstitutionsError] = useState("");
  const institutionOptions = useMemo(
    () =>
      institutions.map((institution) => ({
        value: String(institution.id),
        label: institution.nome,
      })),
    [institutions]
  );

  function goToLogin() {
    if (navigation.canGoBack()) {
      navigation.goBack();
      return;
    }

    navigation.replace("SignIn");
  }

  useEffect(() => {
    let isMounted = true;

    setInstitutionsLoading(true);
    setInstitutionsError("");

    institutionsApi.listPublic()
      .then((items) => {
        if (isMounted) setInstitutions(items);
      })
      .catch((loadError) => {
        if (isMounted) {
          setInstitutions([]);
          setInstitutionsError(
            loadError.message ||
              "Não foi possível carregar as instituições. Verifique se a API está ativa."
          );
        }
      })
      .finally(() => {
        if (isMounted) setInstitutionsLoading(false);
      });

    return () => {
      isMounted = false;
    };
  }, []);

  async function handleSignUp() {
    if (
      !nome.trim() ||
      !email.trim() ||
      !dataNascimento.trim() ||
      !senha ||
      !fotoPerfil ||
      !instituicaoId
    ) {
      setError(
        "Preencha nome, e-mail Fatec, instituição, data, senha e foto de perfil."
      );
      return;
    }

    if (!email.trim().toLowerCase().endsWith("@fatec.sp.gov.br")) {
      setError("Use seu e-mail institucional @fatec.sp.gov.br.");
      return;
    }

    if (senha !== confirmacaoSenha) {
      setError("A confirmação de senha não confere.");
      return;
    }

    setLoading(true);
    setError("");
    setSuccess("");

    try {
      const birthDate = normalizeBrazilianDateInput(dataNascimento);
      await signUp({
        nome,
        email,
        senha,
        dataNascimento: birthDate,
        fotoPerfil,
        instituicaoId,
      });
      setSuccess(
        "Cadastro criado. Enviamos um e-mail de confirmação para seu endereço institucional. Confirme o e-mail antes de fazer login."
      );
      setNome("");
      setEmail("");
      setDataNascimento("");
      setSenha("");
      setConfirmacaoSenha("");
      setFotoPerfil(null);
      setFotoNome("");
      setInstituicaoId("");
    } catch (signUpError) {
      setError(signUpError.message || "Não foi possível criar sua conta.");
    } finally {
      setLoading(false);
    }
  }

  async function handlePickPhoto() {
    setError("");
    setSuccess("");

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
          <AuthHeader
            title="Cadastro"
            onBack={goToLogin}
            right={<ThemeButton theme={theme} toggleTheme={toggleTheme} />}
          />

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

          <Text style={styles.authLabel}>Instituição Fatec</Text>
          <SelectField
            disabled={institutionsLoading || institutionOptions.length === 0}
            modalTitle="Selecione sua FATEC"
            onChange={setInstituicaoId}
            options={institutionOptions}
            placeholder={
              institutionsLoading
                ? "Carregando instituições..."
                : "Selecione sua FATEC"
            }
            value={instituicaoId}
          />

          {institutionsLoading ? (
            <Text style={styles.authMiniWhite}>Carregando instituições...</Text>
          ) : null}

          {!institutionsLoading && institutionsError ? (
            <Text style={styles.authMiniWhite}>{institutionsError}</Text>
          ) : null}

          {!institutionsLoading &&
          !institutionsError &&
          institutions.length === 0 ? (
            <Text style={styles.authMiniWhite}>
              Nenhuma instituição ativa cadastrada disponível no momento.
            </Text>
          ) : null}

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
          {success ? <Text style={styles.authErrorText}>{success}</Text> : null}

          <TouchableOpacity
            style={styles.authSwitchRow}
            onPress={goToLogin}
          >
            <Text style={styles.authMiniWhite}>Já possui uma conta?</Text>
            <Text style={styles.authSwitchLink}> Entrar</Text>
          </TouchableOpacity>

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
