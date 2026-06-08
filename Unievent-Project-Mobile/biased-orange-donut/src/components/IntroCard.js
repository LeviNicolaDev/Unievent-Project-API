import { Text, View } from "react-native";
import { BLACK, ORANGE } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function IntroCard({ title, orange, transparent }) {
  return (
    <View
      style={[
        styles.introCard,
        transparent
          ? styles.introCardTransparent
          : { backgroundColor: orange ? ORANGE : BLACK },
      ]}
    >
      <Text style={styles.introTitle}>{title}</Text>
      <Text style={styles.introText}>
        Crie sua conta e conecte-se à sua instituição para estar sempre
        atualizado com os próximos eventos
      </Text>
    </View>
  );
}
