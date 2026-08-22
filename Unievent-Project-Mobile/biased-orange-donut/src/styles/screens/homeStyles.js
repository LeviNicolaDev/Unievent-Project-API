import { ORANGE } from "../../constants/theme";

export const homeStyles = {
  // HOME
  homeScroll: {
    paddingTop: 42,
    paddingBottom: 125,
  },

  homeHeader: {
    marginBottom: 20,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
  },

  homeLogoImage: {
    width: 125,
    height: 38,
    resizeMode: "contain",
  },

  homeIcons: {
    flexDirection: "row",
    alignItems: "center",
    gap: 18,
  },

  notificationDot: {
    width: 7,
    height: 7,
    borderRadius: 4,
    backgroundColor: ORANGE,
    position: "absolute",
    right: -1,
    top: 1,
  },

  homeHello: {
    fontSize: 21,
    fontWeight: "900",
    marginBottom: 12,
  },

  homeSubtitle: {
    fontSize: 13,
    marginBottom: 22,
    lineHeight: 18,
  },

  homeChips: {
    gap: 9,
    paddingRight: 22,
    marginBottom: 24,
  },

  homeChip: {
    borderWidth: 1,
    borderRadius: 6,
    paddingHorizontal: 12,
    paddingVertical: 8,
  },

  homeChipText: {
    fontSize: 13,
    fontWeight: "600",
  },

  homeFilterSelect: {
    marginBottom: 16,
  },

  homeSection: {
    marginBottom: 14,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
  },

  homeSectionTitle: {
    fontSize: 20,
    fontWeight: "900",
  },

  homeSectionAction: {
    fontSize: 13,
    fontWeight: "500",
    textDecorationLine: "underline",
  },

  nextEventsList: {
    gap: 22,
    paddingRight: 22,
  },

  inviteSpacing: {
    marginTop: 26,
    marginBottom: 26,
    paddingHorizontal: 28,
  },

  allEventsList: {
    gap: 22,
  },
};
