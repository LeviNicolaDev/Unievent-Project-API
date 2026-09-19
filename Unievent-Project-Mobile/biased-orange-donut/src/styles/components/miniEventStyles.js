import { StyleSheet } from "react-native";
import { ORANGE } from "../../constants/theme";

export const miniEventStyles = {
  // MINI EVENT CARD
  miniCard: {
    width: 230,
    borderRadius: 16,
    overflow: "hidden",
    marginRight: 16,
    backgroundColor: "#1F1F1F",
  },

  miniImageWrapper: {
    height: 120,
    position: "relative",
  },

  miniImage: {
    width: "100%",
    height: "100%",
    resizeMode: "cover",
    backgroundColor: "#111111",
  },

  miniOverlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: "rgba(0,0,0,0.35)",
  },

  miniHeart: {
    position: "absolute",
    top: 10,
    right: 10,
  },

  miniContent: {
    padding: 12,
  },

  miniOrganizerRow: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 6,
  },

  miniBadge: {
    width: 18,
    height: 18,
    borderRadius: 3,
    backgroundColor: "#FFFFFF",
    alignItems: "center",
    justifyContent: "center",
    marginRight: 6,
  },

  miniBadgeText: {
    fontSize: 12,
    fontWeight: "900",
    color: ORANGE,
  },

  miniOrganizerText: {
    fontSize: 10,
    color: "#DADADA",
  },

  miniTitle: {
    fontSize: 14,
    fontWeight: "900",
    color: "#FFFFFF",
    marginBottom: 8,
  },

  miniInfoRow: {
    flexDirection: "row",
    justifyContent: "space-between",
  },

  miniInfo: {
    flexDirection: "row",
    alignItems: "center",
    gap: 5,
  },

  miniInfoText: {
    fontSize: 10,
    color: "#DADADA",
  },
};
