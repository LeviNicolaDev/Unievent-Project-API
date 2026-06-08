import { Text, TouchableOpacity } from "react-native";
import { BLACK } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function Button({ title, onPress }) {
  return (
    <TouchableOpacity
      onPress={onPress}
      style={[styles.button, { backgroundColor: BLACK }]}
    >
      <Text style={styles.buttonTxt}>{title}</Text>
    </TouchableOpacity>
  );
}
