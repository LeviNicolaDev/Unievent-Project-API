import { ORANGE } from "../../constants/theme";

export const ticketQrStyles = {
  // QR CODE
  qrScreen: {
    flex: 1,
    paddingTop: 34,
    paddingBottom: 100,
  },

  qrTop: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 35,
  },

  qrHeaderTitle: {
    fontSize: 18,
    fontWeight: "900",
  },

  qrCard: {
    backgroundColor: "#1F1F1F",
    borderRadius: 18,
    padding: 22,
    alignItems: "center",
  },

  qrEventTitle: {
    color: "#FFFFFF",
    fontSize: 20,
    fontWeight: "900",
    marginBottom: 8,
  },

  qrInfo: {
    color: "#DADADA",
    fontSize: 12,
    marginBottom: 4,
  },

  qrBox: {
    backgroundColor: "#FFFFFF",
    padding: 18,
    borderRadius: 16,
    marginTop: 28,
    marginBottom: 18,
  },

  qrCodeText: {
    color: ORANGE,
    fontSize: 13,
    fontWeight: "900",
    marginBottom: 12,
  },

  qrHelpText: {
    color: "#DADADA",
    fontSize: 12,
    textAlign: "center",
    lineHeight: 18,
  },

  qrActions: {
    width: "100%",
    marginTop: 22,
    gap: 12,
  },

  qrReadButton: {
    minHeight: 48,
    borderRadius: 8,
    backgroundColor: ORANGE,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    paddingHorizontal: 14,
  },

  qrReadButtonDisabled: {
    backgroundColor: "#696969",
  },

  qrReadButtonText: {
    color: "#FFFFFF",
    fontSize: 13,
    fontWeight: "900",
    textAlign: "center",
  },

  qrReadBadge: {
    minHeight: 42,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: ORANGE,
    backgroundColor: "#2B231F",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    paddingHorizontal: 14,
  },

  qrReadText: {
    color: "#FFFFFF",
    fontSize: 13,
    fontWeight: "800",
  },

  certificateButton: {
    minHeight: 50,
    borderRadius: 8,
    backgroundColor: ORANGE,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    paddingHorizontal: 14,
  },

  certificateButtonDisabled: {
    backgroundColor: "#696969",
  },

  certificateButtonText: {
    color: "#FFFFFF",
    fontSize: 14,
    fontWeight: "900",
    textAlign: "center",
  },

  certificateUnavailableBadge: {
    minHeight: 44,
    borderRadius: 8,
    backgroundColor: "#393939",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 7,
    paddingHorizontal: 12,
  },

  certificateUnavailableText: {
    color: "#DADADA",
    fontSize: 12,
    fontWeight: "800",
    textAlign: "center",
  },
};
