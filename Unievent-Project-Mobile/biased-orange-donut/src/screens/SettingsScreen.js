import { Ionicons } from "@expo/vector-icons";
import { useState } from "react";
import { ScrollView, Switch, Text, TouchableOpacity, View } from "react-native";

import BottomNav from "../components/BottomNav";
import Screen from "../components/Screen";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function SettingsScreen({
  theme,
  navigation,
  toggleTheme,
  largeText,
  setLargeText,
}) {
  const isLight = theme.mode === "light";
  const [highContrast, setHighContrast] = useState(false);
  const [reduceMotion, setReduceMotion] = useState(true);
  const [screenReaderHints, setScreenReaderHints] = useState(true);
  const [eventReminders, setEventReminders] = useState(true);
  const [certificateAlerts, setCertificateAlerts] = useState(true);
  const [favoriteSuggestions, setFavoriteSuggestions] = useState(true);

  const accessibilityItems = [
    {
      icon: "text-outline",
      title: "Texto maior",
      description: "Aumenta a leitura nas telas principais.",
      value: largeText,
      onValueChange: setLargeText,
    },
    {
      icon: "contrast-outline",
      title: "Alto contraste",
      description: "Realça botões, textos e bordas importantes.",
      value: highContrast,
      onValueChange: setHighContrast,
    },
    {
      icon: "sparkles-outline",
      title: "Reduzir animações",
      description: "Evita transições visuais muito fortes.",
      value: reduceMotion,
      onValueChange: setReduceMotion,
    },
    {
      icon: "ear-outline",
      title: "Dicas para leitor de tela",
      description: "Prioriza labels claros em ações e ingressos.",
      value: screenReaderHints,
      onValueChange: setScreenReaderHints,
    },
  ];

  const notificationItems = [
    {
      icon: "notifications-outline",
      title: "Lembretes de eventos",
      description: "Avise antes dos eventos inscritos começarem.",
      value: eventReminders,
      onValueChange: setEventReminders,
    },
    {
      icon: "document-text-outline",
      title: "Avisos de certificado",
      description: "Notifique quando um certificado puder ser emitido.",
      value: certificateAlerts,
      onValueChange: setCertificateAlerts,
    },
    {
      icon: "heart-outline",
      title: "Sugestões por favoritos",
      description: "Use seus interesses para destacar eventos parecidos.",
      value: favoriteSuggestions,
      onValueChange: setFavoriteSuggestions,
    },
  ];

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.settingsScroll}
      >
        <View style={styles.settingsHeader}>
          <TouchableOpacity
            onPress={() =>
              navigation.canGoBack()
                ? navigation.goBack()
                : navigation.navigate("Profile")
            }
            style={styles.settingsBackButton}
          >
            <Ionicons
              name="arrow-back"
              size={21}
              color={isLight ? BLACK : ORANGE}
            />
          </TouchableOpacity>

          <Text style={[styles.settingsTitle, { color: theme.text }]}>
            Configurações
          </Text>

          <View style={styles.settingsBackButton} />
        </View>

        <View
          style={[
            styles.settingsHero,
            { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
          ]}
        >
          <View style={styles.settingsHeroIcon}>
            <Ionicons name="settings" size={22} color="#FFFFFF" />
          </View>

          <View style={styles.settingsHeroText}>
            <Text style={[styles.settingsHeroTitle, { color: theme.text }]}>
              Preferências do UniEvent
            </Text>
            <Text style={[styles.settingsHeroSubtitle, { color: theme.soft }]}>
              Ajuste notificações, acessibilidade, privacidade e certificados.
            </Text>
          </View>
        </View>

        <SettingsSection title="Aparência" theme={theme}>
          <SettingsToggle
            icon={isLight ? "sunny-outline" : "moon-outline"}
            title="Modo claro"
            description="Alterna o tema visual do aplicativo."
            value={isLight}
            onValueChange={toggleTheme}
            theme={theme}
          />
        </SettingsSection>

        <SettingsSection title="Acessibilidade" theme={theme}>
          {accessibilityItems.map((item) => (
            <SettingsToggle key={item.title} {...item} theme={theme} />
          ))}
        </SettingsSection>

        <SettingsSection title="Notificações" theme={theme}>
          {notificationItems.map((item) => (
            <SettingsToggle key={item.title} {...item} theme={theme} />
          ))}
        </SettingsSection>

        <SettingsSection title="Conta" theme={theme}>
          <SettingsLink
            icon="person-outline"
            title="Editar perfil"
            description="Atualize nome, e-mail e senha."
            onPress={() => navigation.navigate("Profile")}
            theme={theme}
          />
          <SettingsLink
            icon="ticket-outline"
            title="Meus ingressos"
            description="Veja inscrições e QR Codes de validação."
            onPress={() => navigation.navigate("Ticket")}
            theme={theme}
          />
          <SettingsLink
            icon="checkmark-circle-outline"
            title="Meus eventos"
            description="Acompanhe eventos validados e certificados."
            onPress={() => navigation.navigate("MyEvents")}
            theme={theme}
          />
        </SettingsSection>
      </ScrollView>

      <BottomNav routeName="Profile" theme={theme} navigation={navigation} />
    </Screen>
  );
}

function SettingsSection({ title, theme, children }) {
  const isLight = theme.mode === "light";

  return (
    <View style={styles.settingsSection}>
      <Text style={[styles.settingsSectionTitle, { color: theme.text }]}>
        {title}
      </Text>

      <View
        style={[
          styles.settingsSectionBody,
          { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
        ]}
      >
        {children}
      </View>
    </View>
  );
}

function SettingsToggle({
  icon,
  title,
  description,
  value,
  onValueChange,
  theme,
}) {
  const isLight = theme.mode === "light";

  return (
    <View style={styles.settingsRow}>
      <View
        style={[
          styles.settingsRowIcon,
          { backgroundColor: isLight ? "#F2F2F2" : "#2B2B2B" },
        ]}
      >
        <Ionicons name={icon} size={18} color={isLight ? BLACK : ORANGE} />
      </View>

      <View style={styles.settingsRowText}>
        <Text style={[styles.settingsRowTitle, { color: theme.text }]}>
          {title}
        </Text>
        <Text style={[styles.settingsRowDescription, { color: theme.soft }]}>
          {description}
        </Text>
      </View>

      <Switch
        value={value}
        onValueChange={onValueChange}
        trackColor={{ false: "#767577", true: ORANGE }}
        thumbColor="#FFFFFF"
      />
    </View>
  );
}

function SettingsLink({ icon, title, description, onPress, theme }) {
  const isLight = theme.mode === "light";

  return (
    <TouchableOpacity
      activeOpacity={0.85}
      onPress={onPress}
      style={styles.settingsRow}
    >
      <View
        style={[
          styles.settingsRowIcon,
          { backgroundColor: isLight ? "#F2F2F2" : "#2B2B2B" },
        ]}
      >
        <Ionicons name={icon} size={18} color={isLight ? BLACK : ORANGE} />
      </View>

      <View style={styles.settingsRowText}>
        <Text style={[styles.settingsRowTitle, { color: theme.text }]}>
          {title}
        </Text>
        <Text style={[styles.settingsRowDescription, { color: theme.soft }]}>
          {description}
        </Text>
      </View>

      <Ionicons
        name="chevron-forward"
        size={18}
        color={isLight ? BLACK : "#FFFFFF"}
      />
    </TouchableOpacity>
  );
}
