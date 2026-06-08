import { Platform, SafeAreaView } from "react-native";
import { styles } from "../styles/globalStyles";

export default function Screen({ children, theme, bg, pad = true }) {
  return (
    <SafeAreaView
      style={[
        styles.safe,
        {
          backgroundColor: bg || theme.bg,
          paddingHorizontal: pad ? 22 : 0,
          paddingTop: Platform.OS === "android" ? 28 : 0,
        },
      ]}
    >
      {children}
    </SafeAreaView>
  );
}
