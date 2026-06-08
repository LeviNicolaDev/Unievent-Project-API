import { Ionicons } from "@expo/vector-icons";
import { Text, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function Info({ icon, text, theme }) {
  return (
    <View style={styles.info}>
      <Ionicons name={icon} color={ORANGE} size={14} />
      <Text style={[styles.infoText, { color: theme.soft }]}>{text}</Text>
    </View>
  );
}
