import { Image, Share, Text, TouchableOpacity, View } from "react-native";
import TicketIcon from "../../assets/cardIcone.png";
import { styles } from "../styles/globalStyles";

export default function Invite() {
  async function handleShare() {
    try {
      await Share.share({
        message: "Participe dos eventos comigo no UniEvent!",
      });
    } catch {
      return null;
    }
  }

  return (
    <View style={styles.inviteCard}>
      <View style={styles.inviteLeft}>
        <Text style={styles.inviteTitle}>Convide seus amigos</Text>

        <Text style={styles.inviteText}>
          Eventos são ainda melhores com amigos!
        </Text>

        <TouchableOpacity style={styles.inviteButton} onPress={handleShare}>
          <Text style={styles.inviteButtonText}>Compartilhe</Text>
        </TouchableOpacity>
      </View>

      <Image source={TicketIcon} style={styles.inviteIconImage} />
    </View>
  );
}
