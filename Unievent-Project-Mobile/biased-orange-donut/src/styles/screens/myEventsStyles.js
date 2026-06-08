import { ORANGE } from "../../constants/theme";

export const myEventsStyles = {
  // MY EVENTS
  myEventsScroll: {
    paddingTop: 34,
    paddingBottom: 130,
  },

  myEventsHeader: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 24,
  },

  myEventsTitle: {
    fontSize: 18,
    fontWeight: "900",
  },

  myEventsList: {
    gap: 14,
  },

  myEventsEmpty: {
    borderRadius: 8,
    padding: 22,
    alignItems: "center",
  },

  myEventsEmptyTitle: {
    fontSize: 16,
    fontWeight: "900",
    marginTop: 12,
    marginBottom: 6,
  },

  myEventsEmptyText: {
    fontSize: 12,
    lineHeight: 18,
    textAlign: "center",
  },

  myEventCard: {
    borderRadius: 8,
    overflow: "hidden",
  },

  myEventImage: {
    width: "100%",
    height: 128,
    resizeMode: "cover",
    backgroundColor: "#111111",
  },

  myEventContent: {
    padding: 14,
  },

  myEventTop: {
    flexDirection: "row",
    alignItems: "flex-start",
    justifyContent: "space-between",
    gap: 10,
    marginBottom: 8,
  },

  myEventTitle: {
    flex: 1,
    fontSize: 15,
    fontWeight: "900",
    lineHeight: 19,
  },

  myEventValidatedBadge: {
    borderRadius: 8,
    backgroundColor: "#2B231F",
    borderWidth: 1,
    borderColor: ORANGE,
    flexDirection: "row",
    alignItems: "center",
    gap: 5,
    paddingHorizontal: 8,
    paddingVertical: 5,
  },

  myEventValidatedText: {
    color: "#FFFFFF",
    fontSize: 9,
    fontWeight: "900",
  },

  myEventMeta: {
    fontSize: 11,
    lineHeight: 16,
  },

  myEventCertificateButton: {
    minHeight: 44,
    borderRadius: 8,
    backgroundColor: ORANGE,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
    paddingHorizontal: 12,
    marginTop: 14,
  },

  myEventCertificateText: {
    color: "#FFFFFF",
    fontSize: 13,
    fontWeight: "900",
    textAlign: "center",
  },

  myEventNoCertificate: {
    minHeight: 42,
    borderRadius: 8,
    backgroundColor: "#393939",
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 7,
    paddingHorizontal: 12,
    marginTop: 14,
  },

  myEventNoCertificateText: {
    color: "#DADADA",
    fontSize: 12,
    fontWeight: "800",
    textAlign: "center",
  },
};
