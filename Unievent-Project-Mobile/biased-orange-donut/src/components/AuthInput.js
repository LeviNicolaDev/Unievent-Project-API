import { Ionicons } from "@expo/vector-icons";
import { TextInput, View } from "react-native";
import { styles } from "../styles/globalStyles";

export default function AuthInput({
  autoCapitalize = "none",
  icon,
  keyboardType = "default",
  onChangeText,
  placeholder,
  secure,
  theme,
  value,
}) {
  return (
    <View style={[styles.inputBox, { borderColor: theme.border }]}>
      <Ionicons name={icon} color={theme.text} size={15} />

      <TextInput
        autoCapitalize={autoCapitalize}
        keyboardType={keyboardType}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor={theme.muted}
        secureTextEntry={secure}
        style={[styles.input, { color: theme.text }]}
        value={value}
      />

      {secure && (
        <Ionicons name="eye-off-outline" color={theme.text} size={17} />
      )}
    </View>
  );
}
