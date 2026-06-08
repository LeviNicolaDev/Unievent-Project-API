import { Ionicons } from "@expo/vector-icons";
import { Image, Text, TouchableOpacity, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";

export default function MiniEvent({ e, theme, navigation }) {
  const { isFavorite, toggleFavorite } = useEvents();
  const favorite = isFavorite(e.id);

  return (
    <TouchableOpacity
      activeOpacity={0.9}
      onPress={() => navigation.navigate("Details", { eventId: e.id })}
      style={styles.miniCard}
    >
      {/* IMAGEM */}
      <View style={styles.miniImageWrapper}>
        <Image source={e.image} style={styles.miniImage} />

        {/* overlay escuro */}
        <View style={styles.miniOverlay} />

        {/* coração */}
        <TouchableOpacity
          style={styles.miniHeart}
          hitSlop={8}
          onPress={(event) => {
            event.stopPropagation();
            toggleFavorite(e.id);
          }}
        >
          <Ionicons
            name={favorite ? "heart" : "heart-outline"}
            size={18}
            color={ORANGE}
          />
        </TouchableOpacity>
      </View>

      {/* CONTEÚDO */}
      <View style={styles.miniContent}>
        {/* organizador */}
        <View style={styles.miniOrganizerRow}>
          <View style={styles.miniBadge}>
            <Text style={styles.miniBadgeText}>F</Text>
          </View>

          <Text style={styles.miniOrganizerText}>{e.place}</Text>
        </View>

        {/* título */}
        <Text numberOfLines={1} style={styles.miniTitle}>
          {e.title}
        </Text>

        {/* infos */}
        <View style={styles.miniInfoRow}>
          <View style={styles.miniInfo}>
            <Ionicons name="calendar-outline" size={14} color={ORANGE} />
            <Text style={styles.miniInfoText}>{e.date}</Text>
          </View>

          <View style={styles.miniInfo}>
            <Ionicons name="time-outline" size={14} color={ORANGE} />
            <Text style={styles.miniInfoText}>{e.time}</Text>
          </View>
        </View>
      </View>
    </TouchableOpacity>
  );
}
