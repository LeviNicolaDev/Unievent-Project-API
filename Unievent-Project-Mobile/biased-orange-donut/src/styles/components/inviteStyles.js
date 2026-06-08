import { ORANGE } from "../../constants/theme";

export const inviteStyles = {
  // INVITE CARD
  inviteCard: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    backgroundColor: "#1F1F1F",
    borderRadius: 16,
    paddingVertical: 18,
    paddingHorizontal: 18,
  },

  inviteLeft: {
    flex: 1,
    marginRight: 10,
  },

  inviteTitle: {
    color: "#FFFFFF",
    fontSize: 16,
    fontWeight: "900",
    marginBottom: 6,
  },

  inviteText: {
    color: "#CFCFCF",
    fontSize: 12,
    marginBottom: 12,
  },

  inviteButton: {
    backgroundColor: ORANGE,
    paddingVertical: 6,
    paddingHorizontal: 14,
    borderRadius: 6,
    alignSelf: "flex-start",
  },

  inviteButtonText: {
    color: "#FFFFFF",
    fontSize: 12,
    fontWeight: "700",
  },

  inviteIconImage: {
    width: 70,
    height: 70,
    resizeMode: "contain",
  },
};
