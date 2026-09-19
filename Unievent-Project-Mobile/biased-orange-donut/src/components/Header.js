import { Ionicons } from "@expo/vector-icons";
import { Text, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { styles } from "../styles/globalStyles";
import ThemeButton from "./ThemeButton";

export default function Header({ title, subtitle, theme, toggleTheme }) {
  return (
    <View style={styles.header}>
      <View>
        <Text style={[styles.headerTitle, { color: theme.text }]}>{title}</Text>
        <Text style={[styles.headerSub, { color: theme.soft }]}>{subtitle}</Text>
      </View>
      <View style={styles.headIcons}>
        <Ionicons name="notifications-outline" color={ORANGE} size={17} />
        <ThemeButton theme={theme} toggleTheme={toggleTheme} />
      </View>
    </View>
  );
}
