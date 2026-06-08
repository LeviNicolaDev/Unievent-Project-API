import { TouchableOpacity, View } from "react-native";
import Logo from "../components/Logo";
import Screen from "../components/Screen";
import { styles } from "../styles/globalStyles";

export default function LoadScreen({ theme, navigation }) {
  return (
    <TouchableOpacity
      style={{ flex: 1 }}
      onPress={() => navigation.navigate("IntroOne")}
    >
      <Screen theme={theme} bg={theme.splash}>
        <View style={styles.center}>
          <Logo color="#fff" />
        </View>
      </Screen>
    </TouchableOpacity>
  );
}
