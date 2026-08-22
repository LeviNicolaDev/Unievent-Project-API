import { Image, Text, TouchableOpacity, View } from "react-native";
import { useState } from "react";
import LogoPng from "../../assets/Logo.png";
import LogoPretaPng from "../../assets/LogoPreta.png";
import AuthHeader from "../components/AuthHeader";
import AuthInput from "../components/AuthInput";
import Screen from "../components/Screen";
import ThemeButton from "../components/ThemeButton";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { styles } from "../styles/globalStyles";

export default function SignInScreen({ theme, navigation, toggleTheme }) {
  const isLight = theme.mode === "light";
  const logo = isLight ? LogoPretaPng : LogoPng;
  const canGoBack = navigation.canGoBack();
  const { signIn } = useAuth();
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleLogin() {
    if (!email.trim() || !senha) {
      setError("Informe e-mail e senha para entrar.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      await signIn({ email, senha });
      navigation.replace("Home");
    } catch (loginError) {
      setError(loginError.message || "Não foi possível fazer login.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK} pad={false}>
      <View style={styles.authScreen}>
        <View
          style={[
            styles.authTop,
            { backgroundColor: isLight ? LIGHT_BG : BLACK },
          ]}
        >
          <Image source={logo} style={styles.authLogoImage} />
        </View>

        <View
          style={[
            styles.authBottom,
            { backgroundColor: isLight ? BLACK : ORANGE },
          ]}
        >
          <AuthHeader
            title="Login"
            onBack={canGoBack ? () => navigation.goBack() : null}
            right={<ThemeButton theme={theme} toggleTheme={toggleTheme} />}
          />

          <Text style={styles.authLabel}>E-mail</Text>
          <AuthInput
            icon="mail-outline"
            keyboardType="email-address"
            onChangeText={setEmail}
            placeholder="email@fatec.sp.gov.br"
            theme={isLight ? darkInputTheme : orangeInputTheme}
            value={email}
          />

          <Text style={styles.authLabel}>Senha</Text>
          <AuthInput
            icon="lock-closed-outline"
            onChangeText={setSenha}
            placeholder="Insira aqui sua senha"
            secure
            theme={isLight ? darkInputTheme : orangeInputTheme}
            value={senha}
          />

          <View style={styles.authRememberRow}>
            <Text style={styles.authMiniWhite}>□ Lembrar senha</Text>
          </View>

          <TouchableOpacity
            disabled={loading}
            style={[
              styles.signInButton,
              {
                backgroundColor: BLACK,
                borderWidth: isLight ? 1 : 0,
                borderColor: isLight ? "#FFFFFF" : "transparent",
                opacity: loading ? 0.7 : 1,
              },
            ]}
            onPress={handleLogin}
          >
            <Text style={styles.signInButtonText}>
              {loading ? "Entrando..." : "Login"}
            </Text>
          </TouchableOpacity>

          {error ? <Text style={styles.authErrorText}>{error}</Text> : null}

          <View style={styles.authLinksRow}>
            <TouchableOpacity>
              <Text style={styles.authLinkWhite}>Esqueci minha senha</Text>
            </TouchableOpacity>

            <TouchableOpacity onPress={() => navigation.navigate("SignUp")}>
              <Text style={styles.authLinkWhite}>Criar conta</Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    </Screen>
  );
}

const orangeInputTheme = {
  text: "#FFFFFF",
  muted: "#FFFFFF",
  border: "#FFFFFF",
};

const darkInputTheme = {
  text: "#FFFFFF",
  muted: "#FFFFFF",
  border: "#FFFFFF",
};
