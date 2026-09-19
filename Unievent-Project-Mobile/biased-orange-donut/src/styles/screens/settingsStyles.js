import { ORANGE } from "../../constants/theme";

export const settingsStyles = {
  // SETTINGS
  settingsScroll: {
    paddingTop: 34,
    paddingBottom: 130,
  },

  settingsHeader: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 22,
  },

  settingsBackButton: {
    width: 34,
    height: 34,
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
  },

  settingsTitle: {
    fontSize: 18,
    fontWeight: "900",
  },

  settingsHero: {
    borderRadius: 8,
    padding: 16,
    flexDirection: "row",
    alignItems: "center",
    gap: 12,
    marginBottom: 24,
  },

  settingsHeroIcon: {
    width: 46,
    height: 46,
    borderRadius: 8,
    backgroundColor: ORANGE,
    alignItems: "center",
    justifyContent: "center",
  },

  settingsHeroText: {
    flex: 1,
  },

  settingsHeroTitle: {
    fontSize: 15,
    fontWeight: "900",
    marginBottom: 4,
  },

  settingsHeroSubtitle: {
    fontSize: 11,
    lineHeight: 16,
  },

  settingsSection: {
    marginBottom: 18,
  },

  settingsSectionTitle: {
    fontSize: 14,
    fontWeight: "900",
    marginBottom: 9,
  },

  settingsSectionBody: {
    borderRadius: 8,
    overflow: "hidden",
  },

  settingsRow: {
    minHeight: 72,
    flexDirection: "row",
    alignItems: "center",
    gap: 12,
    paddingHorizontal: 13,
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: "rgba(150,150,150,0.18)",
  },

  settingsRowIcon: {
    width: 36,
    height: 36,
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
  },

  settingsRowText: {
    flex: 1,
  },

  settingsRowTitle: {
    fontSize: 13,
    fontWeight: "900",
    marginBottom: 3,
  },

  settingsRowDescription: {
    fontSize: 10,
    lineHeight: 14,
  },
};
