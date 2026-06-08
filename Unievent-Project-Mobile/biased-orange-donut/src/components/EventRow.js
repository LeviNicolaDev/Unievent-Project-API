import { Ionicons } from "@expo/vector-icons";
import { Image, Text, TouchableOpacity, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";

export default function EventRow({ e, navigation }) {
  const { isFavorite, toggleFavorite } = useEvents();
  const favorite = isFavorite(e.id);

  return (
    <TouchableOpacity
      activeOpacity={0.9}
      onPress={() => navigation.navigate("Details", { eventId: e.id })}
      style={styles.rowCard}
    >
      {/* IMAGEM */}
      <View style={styles.rowImageWrapper}>
        <Image source={e.image} style={styles.rowImage} />

        {/* coração */}
        <TouchableOpacity
          style={styles.rowHeart}
          hitSlop={8}
          onPress={(event) => {
            event.stopPropagation();
            toggleFavorite(e.id);
          }}
        >
          <Ionicons
            name={favorite ? "heart" : "heart-outline"}
            size={16}
            color={ORANGE}
          />
        </TouchableOpacity>
      </View>

      {/* CONTEÚDO */}
      <View style={styles.rowContent}>
        <Text numberOfLines={2} style={styles.rowTitle}>
          {e.title}
        </Text>

        <View style={styles.rowInfo}>
          <Ionicons name="calendar-outline" size={14} color={ORANGE} />
          <Text style={styles.rowInfoText}>{e.date}</Text>
        </View>

        <View style={styles.rowInfo}>
          <Ionicons name="time-outline" size={14} color={ORANGE} />
          <Text style={styles.rowInfoText}>{e.time}</Text>
        </View>
      </View>
    </TouchableOpacity>
  );
}
