import { NavigationContainer } from "@react-navigation/native";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { useState } from "react";
import { StatusBar, View } from "react-native";

import { DARK, LIGHT } from "./src/constants/theme";
import { AuthProvider } from "./src/context/AuthContext";
import { EventProvider } from "./src/context/EventContext";

import DetailsScreen from "./src/screens/DetailsScreen";
import FavoritesScreen from "./src/screens/FavoritesScreen";
import HomeScreen from "./src/screens/HomeScreen";
import IntroOneScreen from "./src/screens/IntroOneScreen";
import IntroTwoScreen from "./src/screens/IntroTwoScreen";
import LoadScreen from "./src/screens/LoadScreen";
import MyEventsScreen from "./src/screens/MyEventsScreen";
import ProfileScreen from "./src/screens/ProfileScreen";
import SettingsScreen from "./src/screens/SettingsScreen";
import SignInScreen from "./src/screens/SignInScreen";
import SignUpScreen from "./src/screens/SignUpScreen";
import TicketQrScreen from "./src/screens/TicketQrScreen";
import TicketScreen from "./src/screens/TicketScreen";
import { setLargeTextEnabled } from "./src/styles/globalStyles";

const Stack = createNativeStackNavigator();
const linking = {
  prefixes: ["unievent://"],
  config: {
    screens: {
      SignIn: "login",
    },
  },
};

export default function App() {
  const [isLight, setIsLight] = useState(false);
  const [largeText, setLargeText] = useState(false);
  const theme = isLight ? LIGHT : DARK;
  setLargeTextEnabled(largeText);

  const screenProps = {
    theme,
    toggleTheme: () => setIsLight((value) => !value),
    largeText,
    setLargeText,
  };

  return (
    <View style={{ flex: 1, backgroundColor: theme.bg }}>
      <StatusBar
        barStyle={isLight ? "dark-content" : "light-content"}
        backgroundColor={theme.bg}
      />

      <AuthProvider>
        <EventProvider>
          <NavigationContainer linking={linking}>
            <Stack.Navigator
              initialRouteName="Load"
              screenOptions={{ headerShown: false }}
            >
              <Stack.Screen name="Load">
                {(props) => <LoadScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="IntroOne">
                {(props) => <IntroOneScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="IntroTwo">
                {(props) => <IntroTwoScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="SignIn">
                {(props) => <SignInScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="SignUp">
                {(props) => <SignUpScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="Home">
                {(props) => <HomeScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="Details">
                {(props) => <DetailsScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="Ticket">
                {(props) => <TicketScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="Favorites">
                {(props) => <FavoritesScreen {...props} {...screenProps} />}
              </Stack.Screen>

              <Stack.Screen name="Profile">
                {(props) => <ProfileScreen {...props} {...screenProps} />}
              </Stack.Screen>
              <Stack.Screen name="MyEvents">
                {(props) => <MyEventsScreen {...props} {...screenProps} />}
              </Stack.Screen>
              <Stack.Screen name="Settings">
                {(props) => <SettingsScreen {...props} {...screenProps} />}
              </Stack.Screen>
              <Stack.Screen name="TicketQr">
                {(props) => <TicketQrScreen {...props} {...screenProps} />}
              </Stack.Screen>
            </Stack.Navigator>
          </NavigationContainer>
        </EventProvider>
      </AuthProvider>
    </View>
  );
}
