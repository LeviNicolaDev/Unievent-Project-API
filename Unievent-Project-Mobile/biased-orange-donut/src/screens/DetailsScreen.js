import { Ionicons } from "@expo/vector-icons";
import { useState } from "react";
import { Alert, ScrollView, Text, TouchableOpacity, View } from "react-native";

import BottomNav from "../components/BottomNav";
import EventImage from "../components/EventImage";
import Screen from "../components/Screen";
import ThemeButton from "../components/ThemeButton";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";

export default function DetailsScreen({
  theme,
  navigation,
  route,
  toggleTheme,
}) {
  const isLight = theme.mode === "light";
  const [registering, setRegistering] = useState(false);
  const { student } = useAuth();
  const {
    events,
    getEventById,
    isFavorite,
    toggleFavorite,
    isRegistered,
    registerForEvent,
  } = useEvents();
  const event = getEventById(route.params?.eventId) || events[0];
  const favorite = isFavorite(event.id);
  const registered = isRegistered(event.id);
  const isInstitutionOnly = event.publicoPermitido === "AlunosDaInstituicao";
  const isDifferentInstitution =
    isInstitutionOnly &&
    event.institutionId &&
    student?.instituicaoId &&
    String(event.institutionId) !== String(student.instituicaoId);
  const restrictionMessage = isDifferentInstitution
    ? `Evento exclusivo para alunos da ${event.institutionName}.`
    : "";

  async function handleTicketPress() {
    if (registered) {
      navigation.navigate("TicketQr", { eventId: event.id });
      return;
    }

    if (isDifferentInstitution) {
      Alert.alert("Inscrição indisponível", restrictionMessage);
      return;
    }

    setRegistering(true);

    try {
      await registerForEvent(event.id);
      navigation.navigate("TicketQr", { eventId: event.id });
    } catch (error) {
      Alert.alert("Não foi possível garantir ingresso", error.message);
      if (String(error.message).includes("login")) {
        navigation.navigate("SignIn");
      }
    } finally {
      setRegistering(false);
    }
  }

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.detailsScroll}
      >
        <View style={styles.detailsTop}>
          <TouchableOpacity
            onPress={() =>
              navigation.canGoBack() ? navigation.goBack() : navigation.navigate("Home")
            }
          >
            <Ionicons
              name="arrow-back"
              color={isLight ? BLACK : ORANGE}
              size={20}
            />
          </TouchableOpacity>

          <View style={styles.headIcons}>
            <ThemeButton theme={theme} toggleTheme={toggleTheme} />
            <TouchableOpacity onPress={() => toggleFavorite(event.id)}>
              <Ionicons
                name={favorite ? "heart" : "heart-outline"}
                color={ORANGE}
                size={18}
              />
            </TouchableOpacity>
          </View>
        </View>

        <EventImage e={event} big />

        <Text style={[styles.detailsTitle, { color: theme.text }]}>
          {event.title}
        </Text>

        <View style={styles.detailsInfoRow}>
          <Info
            icon="calendar-outline"
            text={event.fullDate || event.date}
            isLight={isLight}
          />

          <Info
            icon="time-outline"
            text={event.displayTime || event.time}
            isLight={isLight}
          />
        </View>

        <Info icon="bookmark" text={event.category} isLight={isLight} />
        <Info
          icon="business-outline"
          text={`Instituição: ${event.institutionName}`}
          isLight={isLight}
        />
        {event.local ? (
          <Info icon="location-outline" text={`Local: ${event.local}`} isLight={isLight} />
        ) : null}
        <Info
          icon="people-outline"
          text={`Público: ${event.audienceLabel || "Público geral"}`}
          isLight={isLight}
        />

        <Text style={[styles.organizerLabel, { color: theme.text }]}>
          Organizador
        </Text>

        <View style={styles.organizerRow}>
          <View style={styles.organizerAvatar}>
            <Text style={styles.organizerAvatarText}>F</Text>
          </View>

          <Text style={[styles.organizerName, { color: theme.text }]}>
            {event.organizer || event.place}
          </Text>
        </View>

        <Text style={[styles.aboutTitle, { color: theme.text }]}>
          Sobre o evento
        </Text>

        <Text style={[styles.aboutText, { color: theme.soft }]}>
          {event.description}
        </Text>

        <TouchableOpacity
          disabled={registering || isDifferentInstitution}
          style={[
            styles.garantedButton,
            {
              backgroundColor: isLight ? BLACK : ORANGE,
              opacity: registering || isDifferentInstitution ? 0.7 : 1,
            },
          ]}
          onPress={handleTicketPress}
        >
          <Text style={styles.garantedText}>
            {isDifferentInstitution
              ? "Inscrição indisponível"
              : registering
              ? "Garantindo..."
              : registered
              ? "Ingresso garantido"
              : "Garantir ingresso"}
          </Text>
        </TouchableOpacity>
        {restrictionMessage ? (
          <Text style={[styles.aboutText, { color: theme.soft }]}>
            {restrictionMessage}
          </Text>
        ) : null}
      </ScrollView>
      <BottomNav navigation={navigation} routeName={route.name} theme={theme} />
    </Screen>
  );
}

function Info({ icon, text, isLight }) {
  return (
    <View style={styles.detailsInfo}>
      <Ionicons name={icon} color={isLight ? BLACK : ORANGE} size={14} />

      <Text
        style={[styles.detailsInfoText, { color: isLight ? BLACK : "#DADADA" }]}
      >
        {text}
      </Text>
    </View>
  );
}
