import { Ionicons } from "@expo/vector-icons";
import { Image, ScrollView, Text, TouchableOpacity, View } from "react-native";

import Unievent from "../../assets/unievent.png";
import UnieventPreta from "../../assets/unieventPreta.png";
import BottomNav from "../components/BottomNav";
import EventRow from "../components/EventRow";
import Invite from "../components/Invite";
import MiniEvent from "../components/MiniEvent";
import Screen from "../components/Screen";
import SelectField from "../components/SelectField";
import ThemeButton from "../components/ThemeButton";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";

export default function HomeScreen({ theme, navigation, route, toggleTheme }) {
  const isLight = theme.mode === "light";
  const { student } = useAuth();
  const {
    categoryFilters,
    eventsError,
    eventsLoading,
    filteredEvents,
    selectedCategory,
    selectedInstitutionId,
    setSelectedInstitutionId,
    institutionFilters,
    setSelectedCategory,
    setSearchTerm,
  } = useEvents();

  const nextEvents = filteredEvents.slice(0, 2);
  const allEvents = filteredEvents.slice(2);
  const institutionOptions = [
    { value: "", label: "Todas as instituições" },
    ...institutionFilters.map((institution) => ({
      value: String(institution.id),
      label: institution.nome,
    })),
  ];

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.homeScroll}
      >
        <View style={styles.homeHeader}>
          <Image
            source={isLight ? UnieventPreta : Unievent}
            style={styles.homeLogoImage}
          />

          <View style={styles.homeIcons}>
            <TouchableOpacity>
              <Ionicons
                name="notifications"
                color={isLight ? BLACK : ORANGE}
                size={20}
              />
              <View style={styles.notificationDot} />
            </TouchableOpacity>

            <ThemeButton theme={theme} toggleTheme={toggleTheme} />
          </View>
        </View>

        <Text style={[styles.homeHello, { color: theme.text }]}>
          Olá, {student?.nome || "Aluno"}!
        </Text>

        <Text style={[styles.homeSubtitle, { color: theme.soft }]}>
          {eventsLoading
            ? "Carregando eventos..."
            : "Bem-vindo! Conecte-se, participe e aproveite ao máximo!"}
        </Text>

        {eventsError ? (
          <Text style={[styles.homeSubtitle, { color: theme.soft }]}>
            Mostrando eventos locais enquanto a API não responde.
          </Text>
        ) : null}

        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={styles.homeChips}
        >
          {categoryFilters.map((item) => {
            const active = item === selectedCategory;

            return (
              <TouchableOpacity
                key={item}
                onPress={() => setSelectedCategory(item)}
                style={[
                  styles.homeChip,
                  {
                    backgroundColor: active
                      ? isLight
                        ? BLACK
                        : ORANGE
                      : "transparent",

                    borderColor: isLight ? BLACK : ORANGE,
                  },
                ]}
              >
                <Text
                  style={[
                    styles.homeChipText,
                    {
                      color: active ? "#FFFFFF" : isLight ? BLACK : "#FFFFFF",
                    },
                  ]}
                >
                  {item}
                </Text>
              </TouchableOpacity>
            );
          })}
        </ScrollView>

        {institutionFilters.length > 0 ? (
          <View style={styles.homeFilterSelect}>
            <Text style={[styles.authLabel, { color: theme.text }]}>
              Instituição
            </Text>
            <SelectField
              modalTitle="Selecione a instituição"
              onChange={setSelectedInstitutionId}
              options={institutionOptions}
              placeholder="Todas as instituições"
              value={selectedInstitutionId}
              variant={isLight ? "orange" : "dark"}
            />
          </View>
        ) : null}
        <View style={styles.homeSection}>
          <Text style={[styles.homeSectionTitle, { color: theme.text }]}>
            Próximos eventos
          </Text>

          <TouchableOpacity
            onPress={() => {
              setSelectedCategory("Todos");
              setSearchTerm("");
            }}
          >
            <Text
              style={[
                styles.homeSectionAction,
                { color: isLight ? BLACK : ORANGE },
              ]}
            >
              Ver mais
            </Text>
          </TouchableOpacity>
        </View>

        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={styles.nextEventsList}
        >
          {nextEvents.map((event) => (
            <MiniEvent
              key={event.id}
              e={event}
              theme={theme}
              navigation={navigation}
            />
          ))}
        </ScrollView>

        <View style={styles.inviteSpacing}>
          <Invite theme={theme} />
        </View>

        <View style={styles.homeSection}>
          <Text style={[styles.homeSectionTitle, { color: theme.text }]}>
            Todos eventos
          </Text>
        </View>

        <View style={styles.allEventsList}>
          {allEvents.map((event) => (
            <EventRow
              key={event.id}
              e={event}
              theme={theme}
              navigation={navigation}
            />
          ))}
        </View>
      </ScrollView>

      <BottomNav navigation={navigation} routeName={route.name} theme={theme} />
    </Screen>
  );
}
