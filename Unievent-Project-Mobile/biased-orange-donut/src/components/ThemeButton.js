import { Ionicons } from "@expo/vector-icons";
import { TouchableOpacity } from "react-native";
import { ORANGE } from "../constants/theme";

export default function ThemeButton({ theme, toggleTheme }) {
  if (theme.mode === "dark") {
    return (
      <TouchableOpacity onPress={toggleTheme}>
        <Ionicons name="sunny-outline" size={18} color={"#ffff"} />
      </TouchableOpacity>
    );
  }
  if (theme.mode === "light") {
    return (
      <TouchableOpacity onPress={toggleTheme}>
        <Ionicons name="moon-outline" size={18} color={ORANGE} />
      </TouchableOpacity>
    );
  }
}
