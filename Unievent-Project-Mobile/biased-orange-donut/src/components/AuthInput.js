import { Ionicons } from "@expo/vector-icons";
import { useState } from "react";
import { TextInput, TouchableOpacity, View } from "react-native";
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
  const [passwordVisible, setPasswordVisible] = useState(false);
  const shouldHideText = Boolean(secure && !passwordVisible);

  return (
    <View style={[styles.inputBox, { borderColor: theme.border }]}>
      <Ionicons name={icon} color={theme.text} size={15} />

      <TextInput
        autoCapitalize={autoCapitalize}
        keyboardType={keyboardType}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor={theme.muted}
        secureTextEntry={shouldHideText}
        style={[styles.input, { color: theme.text }]}
        value={value}
      />

      {secure && (
        <TouchableOpacity
          accessibilityLabel={
            passwordVisible ? "Ocultar senha" : "Visualizar senha"
          }
          accessibilityRole="button"
          hitSlop={10}
          onPress={() => setPasswordVisible((visible) => !visible)}
        >
          <Ionicons
            name={passwordVisible ? "eye-outline" : "eye-off-outline"}
            color={theme.text}
            size={17}
          />
        </TouchableOpacity>
      )}
    </View>
  );
}
