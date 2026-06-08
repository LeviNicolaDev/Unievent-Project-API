import { Ionicons } from "@expo/vector-icons";
import { useState } from "react";
import { ScrollView, Text, TouchableOpacity, View } from "react-native";
import BottomNav from "../components/BottomNav";
import Screen from "../components/Screen";
import TicketCard from "../components/TicketCard";
import { BLACK, LIGHT_BG, ORANGE } from "../constants/theme";
import { useEvents } from "../context/EventContext";
import { categoryFilterMap } from "../data/events";
import { styles } from "../styles/globalStyles";
import { filterEventsByCategory } from "../utils/eventFilters";

export default function TicketScreen({ theme, navigation, route }) {
  const isLight = theme.mode === "light";
  const { registeredEvents, categoryFilters } = useEvents();
  const [filterOpen, setFilterOpen] = useState(false);
  const [selectedFilter, setSelectedFilter] = useState("Todos");
  const filteredEvents = filterEventsByCategory(
    registeredEvents,
    selectedFilter,
    categoryFilterMap
  );

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK}>
      <ScrollView
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.ticketScreenScroll}
      >
        <View style={styles.ticketHeader}>
          <Text style={[styles.ticketHeaderTitle, { color: theme.text }]}>
            Meus Ingressos
          </Text>

          <TouchableOpacity
            activeOpacity={0.85}
            onPress={() => setFilterOpen((value) => !value)}
            style={[
              styles.ticketFilter,
              filterOpen && styles.ticketFilterActive,
            ]}
          >
            <Text style={[styles.ticketFilterText, { color: theme.soft }]}>
              {selectedFilter === "Todos" ? "Filtrar" : selectedFilter}
            </Text>
            <Ionicons name="filter" size={18} color={ORANGE} />
          </TouchableOpacity>
        </View>

        {filterOpen && (
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.ticketFilterOptions}
          >
            {categoryFilters.map((filter) => {
              const active = selectedFilter === filter;

              return (
                <TouchableOpacity
                  key={filter}
                  activeOpacity={0.85}
                  onPress={() => setSelectedFilter(filter)}
                  style={[
                    styles.ticketFilterChip,
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
                      styles.ticketFilterChipText,
                      {
                        color: active ? "#FFFFFF" : isLight ? BLACK : "#FFFFFF",
                      },
                    ]}
                  >
                    {filter}
                  </Text>
                </TouchableOpacity>
              );
            })}
          </ScrollView>
        )}

        <View style={styles.ticketList}>
          {filteredEvents.length > 0 ? (
            filteredEvents.map((event) => (
              <TicketCard
                key={event.id}
                e={event}
                navigation={navigation}
                ticket
              />
            ))
          ) : (
            <View
              style={[
                styles.ticketEmptyState,
                { backgroundColor: isLight ? "#FFFFFF" : "#1F1F1F" },
              ]}
            >
              <Ionicons name="ticket-outline" size={28} color={ORANGE} />
              <Text style={[styles.ticketEmptyTitle, { color: theme.text }]}>
                {registeredEvents.length === 0
                  ? "Nenhum ingresso ainda"
                  : "Nenhum ingresso encontrado"}
              </Text>
              <Text style={[styles.ticketEmptyText, { color: theme.soft }]}>
                {registeredEvents.length === 0
                  ? "Garanta ingresso em um evento para ele aparecer aqui."
                  : "Tente selecionar outra categoria no filtro."}
              </Text>
            </View>
          )}
        </View>
      </ScrollView>

      <BottomNav routeName={route.name} theme={theme} navigation={navigation} />
    </Screen>
  );
}
