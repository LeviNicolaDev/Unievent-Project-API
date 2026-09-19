import { ORANGE } from "../../constants/theme";

export const detailsStyles = {
  // DETAILS
  detailsScroll: {
    paddingTop: 34,
    paddingBottom: 118,
  },

  detailsTop: {
    marginBottom: 24,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
  },

  eventBig: {
    width: "100%",
    height: 235,
    borderRadius: 12,
    marginBottom: 14,
    resizeMode: "cover",
    backgroundColor: "#111111",
  },

  detailsTitle: {
    fontSize: 16,
    fontWeight: "900",
    marginTop: 18,
    marginBottom: 14,
  },

  detailsInfoRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 20,
    marginBottom: 10,
  },

  detailsInfo: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 10,
  },

  detailsInfoText: {
    fontSize: 10,
    marginLeft: 7,
    fontWeight: "500",
  },

  organizerLabel: {
    fontSize: 9,
    fontWeight: "500",
    marginTop: 6,
    marginBottom: 8,
  },

  organizerRow: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 24,
  },

  organizerAvatar: {
    width: 27,
    height: 27,
    borderRadius: 3,
    backgroundColor: "#FFFFFF",
    alignItems: "center",
    justifyContent: "center",
    marginRight: 9,
  },

  organizerAvatarText: {
    color: ORANGE,
    fontSize: 17,
    fontWeight: "900",
  },

  organizerName: {
    flex: 1,
    fontSize: 10,
    fontWeight: "600",
  },

  followButton: {
    paddingHorizontal: 9,
    paddingVertical: 5,
    borderRadius: 3,
  },

  followButtonText: {
    color: "#FFFFFF",
    fontSize: 8,
    fontWeight: "800",
  },
  garantedButton: {
    height: 45,
    alignItems: "center",
    justifyContent: "center",
    borderRadius: 5,
  },
  garantedText: { color: "#FFFFFF", fontSize: 15, fontWeight: "800" },
  aboutTitle: {
    fontSize: 14,
    fontWeight: "900",
    marginBottom: 10,
  },

  aboutText: {
    fontSize: 11,
    lineHeight: 17,
    marginBottom: 24,
  },
};
