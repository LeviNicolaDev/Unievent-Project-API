import { Image, View } from "react-native";
import LogoPng from "../../assets/Logo.png";
import { styles } from "../styles/globalStyles";

export default function Logo({}) {
  return (
    <View style={styles.logoRow}>
      <Image source={LogoPng} style={styles.logoImage} />
    </View>
  );
}
