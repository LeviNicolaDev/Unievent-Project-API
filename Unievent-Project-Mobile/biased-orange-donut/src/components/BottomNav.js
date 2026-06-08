import { Ionicons } from "@expo/vector-icons";
import { TouchableOpacity, View } from "react-native";
import { BLACK } from "../constants/theme";
import { styles } from "../styles/globalStyles";

export default function BottomNav({ navigation, routeName, theme }) {
  const items = [
    ["Home", "home"],
    ["Ticket", "ticket"],
    ["Favorites", "heart"],
    ["Profile", "person"],
  ];

  return (
    <View style={[styles.nav, { backgroundColor: theme.nav }]}>
      {items.map(([screenName, icon]) => {
        const active = routeName === screenName;

        return (
          <TouchableOpacity
            key={screenName}
            onPress={() => navigation.navigate(screenName)}
            style={[
              styles.navItem,
              { backgroundColor: active ? "#fff" : theme.card2 },
            ]}
          >
            <Ionicons
              name={active ? icon : `${icon}-outline`}
              color={active ? BLACK : "#fff"}
              size={19}
            />
          </TouchableOpacity>
        );
      })}
    </View>
  );
}
