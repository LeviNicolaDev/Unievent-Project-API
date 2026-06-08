import { Ionicons } from "@expo/vector-icons";
import { Image, Text, TouchableOpacity, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { useEvents } from "../context/EventContext";
import { styles } from "../styles/globalStyles";

export default function TicketCard({ e, favorite, navigation, ticket }) {
  const { isFavorite, toggleFavorite } = useEvents();
  const liked = favorite || isFavorite(e.id);
  const routeName = ticket ? "TicketQr" : "Details";

  return (
    <TouchableOpacity
      activeOpacity={0.9}
      disabled={!navigation}
      onPress={() => navigation?.navigate(routeName, { eventId: e.id })}
      style={styles.ticketCardCustom}
    >
      <Image source={e.image} style={styles.ticketCardImage} />

      <View style={styles.ticketCardContent}>
        <View style={styles.ticketCardTop}>
          <Text numberOfLines={1} style={styles.ticketCardTitle}>
            {e.title}
          </Text>
          <TouchableOpacity
            hitSlop={8}
            onPress={(event) => {
              event.stopPropagation();
              toggleFavorite(e.id);
            }}
          >
            <Ionicons
              name={liked ? "heart" : "heart-outline"}
              size={12}
              color={ORANGE}
            />
          </TouchableOpacity>
        </View>

        <Text style={styles.ticketCardInfo}>Data</Text>
        <Text style={styles.ticketCardInfo}>{e.date}</Text>

        <Text style={styles.ticketCardInfo}>Horário</Text>
        <Text style={styles.ticketCardInfo}>{e.time}</Text>
      </View>
    </TouchableOpacity>
  );
}
