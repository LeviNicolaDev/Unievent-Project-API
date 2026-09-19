import { Text, TouchableOpacity, View } from "react-native";
import { styles } from "../styles/globalStyles";
import Dots from "./Dots";

export default function IntroFooter({ theme, active, onNext, lightDots }) {
  return (
    <View style={styles.introFooterCustom}>
      <View style={{ width: 55 }} />

      <Dots active={active} theme={theme} lightDots={lightDots} />

      <TouchableOpacity onPress={onNext}>
        <Text style={styles.introFooterTextWhite}>Próximo</Text>
      </TouchableOpacity>
    </View>
  );
}
