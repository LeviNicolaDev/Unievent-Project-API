import { View } from "react-native";
import { styles } from "../styles/globalStyles";

export default function Dots({ active, theme, lightDots }) {
  const isLight = theme.mode === "light";

  return (
    <View style={styles.dots}>
      {[0, 1].map((i) => (
        <View
          key={i}
          style={[
            styles.dot,
            {
              backgroundColor:
                active === i ? "#FFFFFF" : isLight ? "#777777" : "#53331F",
            },
          ]}
        />
      ))}
    </View>
  );
}
