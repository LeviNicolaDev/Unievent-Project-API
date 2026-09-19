import { StyleSheet } from "react-native";

import { bottomNavStyles } from "./components/bottomNavStyles";
import { buttonStyles } from "./components/buttonStyles";
import { eventRowStyles } from "./components/eventRowStyles";
import { inviteStyles } from "./components/inviteStyles";
import { miniEventStyles } from "./components/miniEventStyles";
import { authStyles } from "./screens/authStyles";
import { detailsStyles } from "./screens/detailsStyles";
import { homeStyles } from "./screens/homeStyles";
import { introStyles } from "./screens/introStyles";
import { myEventsStyles } from "./screens/myEventsStyles";
import { profileStyles } from "./screens/profileStyles";
import { settingsStyles } from "./screens/settingsStyles";
import { ticketQrStyles } from "./screens/ticketQrStyles";
import { ticketStyles } from "./screens/ticketStyles";
import { baseStyles as sharedBaseStyles } from "./shared/baseStyles";

const LARGE_TEXT_MULTIPLIER = 1.18;
let largeTextEnabled = false;
const scaledStyleCache = new Map();

export function setLargeTextEnabled(enabled) {
  largeTextEnabled = Boolean(enabled);
}

function getScaledStyle(styleName, style) {
  const flatStyle = StyleSheet.flatten(style);

  if (!flatStyle || typeof flatStyle !== "object") {
    return style;
  }

  if (!flatStyle.fontSize || typeof flatStyle.fontSize !== "number") {
    return style;
  }

  const cacheKey = `${styleName}:${flatStyle.fontSize}`;

  if (!scaledStyleCache.has(cacheKey)) {
    scaledStyleCache.set(cacheKey, {
      ...flatStyle,
      fontSize: Math.round(flatStyle.fontSize * LARGE_TEXT_MULTIPLIER),
    });
  }

  return scaledStyleCache.get(cacheKey);
}

const baseStyles = StyleSheet.create({
  ...sharedBaseStyles,
  ...introStyles,
  ...authStyles,
  ...buttonStyles,
  ...homeStyles,
  ...miniEventStyles,
  ...eventRowStyles,
  ...inviteStyles,
  ...detailsStyles,
  ...ticketStyles,
  ...profileStyles,
  ...settingsStyles,
  ...myEventsStyles,
  ...ticketQrStyles,
  ...bottomNavStyles,
});

export const styles = new Proxy(baseStyles, {
  get(target, styleName) {
    const style = target[styleName];
    return largeTextEnabled ? getScaledStyle(styleName, style) : style;
  },
});
