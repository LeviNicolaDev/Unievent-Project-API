import { Image } from "react-native";
import { styles } from "../styles/globalStyles";

export default function EventImage({ e, big }) {
  return <Image source={e.image} style={big ? styles.eventBig : styles.eventImg} resizeMode="cover" />;
}
