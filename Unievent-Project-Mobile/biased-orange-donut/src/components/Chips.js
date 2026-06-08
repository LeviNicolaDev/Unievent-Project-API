import { Text, View } from "react-native";
import { ORANGE } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function Chips({ theme }) {
  return (
    <View style={styles.chips}>
      {["Todos", "Palestras", "Cursos", "Música", "Workshop"].map((c, i) => (
        <View
          key={c}
          style={[
            styles.chip,
            {
              backgroundColor: i === 0 ? ORANGE : "transparent",
              borderColor: theme.border,
            },
          ]}
        >
          <Text style={{ fontSize: 9, color: i === 0 ? "#fff" : theme.text }}>{c}</Text>
        </View>
      ))}
    </View>
  );
}
