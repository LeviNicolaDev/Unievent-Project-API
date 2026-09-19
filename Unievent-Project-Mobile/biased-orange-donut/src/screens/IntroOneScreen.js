import { Image, View } from "react-native";
import IntroClaro from "../../assets/IntroClaro.png";
import IntroEscuro from "../../assets/IntroEscuro.png";
import IntroCard from "../components/IntroCard";
import IntroFooter from "../components/IntroFooter";
import Screen from "../components/Screen";
import { BLACK, LIGHT_BG } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function IntroOneScreen({ theme, navigation }) {
  const isLight = theme.mode === "light";
  const introImage = isLight ? IntroClaro : IntroEscuro;

  return (
    <Screen theme={theme} bg={isLight ? LIGHT_BG : BLACK} pad={false}>
      <View style={styles.introScreen}>
        <View
          style={[
            styles.introTop,
            { backgroundColor: isLight ? LIGHT_BG : BLACK },
          ]}
        >
          <Image source={introImage} style={styles.introOneImage} />
        </View>

        <View
          style={[
            styles.introBottom,
            { backgroundColor: isLight ? BLACK : "#F56F22" },
          ]}
        >
          <IntroCard
            title="Descubra os próximos eventos da sua faculdade"
            transparent
          />

          <IntroFooter
            theme={theme}
            active={0}
            onNext={() => navigation.navigate("IntroTwo")}
            lightDots={!isLight}
          />
        </View>
      </View>
    </Screen>
  );
}
