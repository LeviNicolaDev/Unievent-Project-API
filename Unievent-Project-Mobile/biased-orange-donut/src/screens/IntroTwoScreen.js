import { Image, Text, TouchableOpacity, View } from "react-native";
import GroupClaro from "../../assets/GroupClaro.png";
import GroupEscuro from "../../assets/GroupEscuro.png";
import Dots from "../components/Dots";
import IntroCard from "../components/IntroCard";
import Screen from "../components/Screen";
import { BLACK, LIGHT_BG } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function IntroTwoScreen({ theme, navigation }) {
  const isLight = theme.mode === "light";
  const introImage = isLight ? GroupClaro : GroupEscuro;

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK} pad={false}>
      <View style={styles.introScreen}>
        <View
          style={[
            styles.introTop,
            { backgroundColor: isLight ? LIGHT_BG : BLACK },
          ]}
        >
          <Image source={introImage} style={styles.introTwoImage} />
        </View>

        <View
          style={[
            styles.introBottom,
            { backgroundColor: isLight ? BLACK : "#F56F22" },
          ]}
        >
          <IntroCard
            title="Não perca nenhum evento da sua faculdade"
            transparent
          />

          <View style={styles.introFooterCustom}>
            <TouchableOpacity onPress={() => navigation.goBack("IntroOne")}>
              <Text style={styles.introFooterTextWhite}>Anterior</Text>
            </TouchableOpacity>

            <Dots active={1} theme={theme} lightDots={!isLight} />

            <TouchableOpacity onPress={() => navigation.navigate("SignIn")}>
              <Text style={styles.introFooterTextWhite}>Próximo</Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    </Screen>
  );
}
