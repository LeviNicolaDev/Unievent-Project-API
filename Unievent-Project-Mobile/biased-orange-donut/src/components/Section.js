import { Text, View } from "react-native";
import { styles } from "../styles/globalStyles";

export default function Section({ title, action, theme }) {
  return (
    <View style={styles.section}>
      <Text style={[styles.secTitle, { color: theme.text }]}>{title}</Text>
      <Text style={styles.secAction}>{action}</Text>
    </View>
  );
}
