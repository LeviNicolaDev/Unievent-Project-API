import { Ionicons } from "@expo/vector-icons";
import { Text, TouchableOpacity, View } from "react-native";

import { styles } from "../styles/globalStyles";

export default function AuthHeader({ title, onBack, right }) {
  return (
    <View style={styles.authHeader}>
      {onBack ? (
        <TouchableOpacity
          accessibilityLabel="Voltar"
          onPress={onBack}
          style={styles.authHeaderBack}
        >
          <Ionicons name="arrow-back" color="#FFFFFF" size={22} />
        </TouchableOpacity>
      ) : (
        <View style={styles.authHeaderBack} />
      )}

      <Text style={styles.authHeaderTitle}>{title}</Text>

      <View style={styles.authHeaderRight}>{right}</View>
    </View>
  );
}
