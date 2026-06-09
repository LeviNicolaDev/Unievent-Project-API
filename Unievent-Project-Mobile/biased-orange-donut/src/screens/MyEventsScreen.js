import { Ionicons } from "@expo/vector-icons";
import { useState } from "react";
import {
  Alert,
  Image,
  ScrollView,
  Text,
  TouchableOpacity,
  View,
} from "react-native";

import BottomNav from "../components/BottomNav";
import Screen from "../components/Screen";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";
import { generateCertificatePdf } from "../utils/certificatePdf";

export default function MyEventsScreen({ theme, navigation }) {
  const isLight = theme.mode === "light";
  const { student } = useAuth();
  const { attendedEvents, hasIssuedCertificate, issueCertificate } = useEvents();
  const [generatingCertificateId, setGeneratingCertificateId] = useState(null);

  async function handleCertificatePress(event) {
    setGeneratingCertificateId(event.id);

    try {
      const certificate = event.certificate;

      await generateCertificatePdf({
        certificate,
        event,
        student,
      });

      issueCertificate(event.id);

      Alert.alert("Certificado gerado", "O PDF do certificado foi gerado.");
    } catch (error) {
      Alert.alert("Certificado indisponível", error.message);
    } finally {
      setGeneratingCertificateId(null);
    }
  }

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.myEventsScroll}
      >
        <View style={styles.myEventsHeader}>
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

          <Text style={[styles.myEventsTitle, { color: theme.text }]}>
            Meus eventos
          </Text>

          <View style={styles.settingsBackButton} />
        </View>

        {attendedEvents.length === 0 ? (
          <View
            style={[
              styles.myEventsEmpty,
              { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
            ]}
          >
            <Ionicons name="scan-outline" size={34} color={ORANGE} />
            <Text style={[styles.myEventsEmptyTitle, { color: theme.text }]}>
              Nenhum evento validado
            </Text>
            <Text style={[styles.myEventsEmptyText, { color: theme.soft }]}>
              Os eventos aparecem aqui somente depois que o QR Code do ingresso
              for verificado.
            </Text>
          </View>
        ) : (
          <View style={styles.myEventsList}>
            {attendedEvents.map((event) => {
              const issued = hasIssuedCertificate(event.id);
              const generating = generatingCertificateId === event.id;

              return (
                <View
                  key={event.id}
                  style={[
                    styles.myEventCard,
                    { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
                  ]}
                >
                  <Image source={event.image} style={styles.myEventImage} />

                  <View style={styles.myEventContent}>
                    <View style={styles.myEventTop}>
                      <Text
                        numberOfLines={2}
                        style={[styles.myEventTitle, { color: theme.text }]}
                      >
                        {event.title}
                      </Text>

                      <View style={styles.myEventValidatedBadge}>
                        <Ionicons
                          name="checkmark-circle"
                          size={14}
                          color={ORANGE}
                        />
                        <Text style={styles.myEventValidatedText}>
                          Participou
                        </Text>
                      </View>
                    </View>

                    <Text style={[styles.myEventMeta, { color: theme.soft }]}>
                      {event.fullDate || event.date} •{" "}
                      {event.displayTime || event.time}
                    </Text>
                    <Text style={[styles.myEventMeta, { color: theme.soft }]}>
                      {event.location || event.place}
                    </Text>

                    {event.hasCertificate && event.certificate ? (
                      <TouchableOpacity
                        activeOpacity={0.85}
                        disabled={generating}
                        onPress={() => handleCertificatePress(event)}
                        style={[
                          styles.myEventCertificateButton,
                          generating && styles.certificateButtonDisabled,
                        ]}
                      >
                        <Ionicons
                          name={
                            issued ? "document-text-outline" : "ribbon-outline"
                          }
                          size={17}
                          color="#FFFFFF"
                        />
                        <Text style={styles.myEventCertificateText}>
                          {generating
                            ? "Gerando PDF..."
                            : issued
                            ? "Abrir certificado"
                            : "Emitir certificado"}
                        </Text>
                      </TouchableOpacity>
                    ) : (
                      <View style={styles.myEventNoCertificate}>
                        <Ionicons
                          name="information-circle-outline"
                          size={16}
                          color="#DADADA"
                        />
                        <Text style={styles.myEventNoCertificateText}>
                          Sem certificado disponível
                        </Text>
                      </View>
                    )}
                  </View>
                </View>
              );
            })}
          </View>
        )}
      </ScrollView>

      <BottomNav routeName="Profile" theme={theme} navigation={navigation} />
    </Screen>
  );
}
