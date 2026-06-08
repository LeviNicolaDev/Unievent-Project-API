export const ticketStyles = {
  // TICKET / FAVORITES
  ticketScreenScroll: {
    paddingTop: 58,
    paddingBottom: 110,
  },

  ticketHeader: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 20,
  },

  ticketHeaderTitle: {
    fontSize: 15,
    fontWeight: "900",
  },

  ticketFilter: {
    flexDirection: "row",
    alignItems: "center",
    gap: 4,
    minHeight: 34,
    borderRadius: 8,
    paddingHorizontal: 8,
  },

  ticketFilterText: {
    fontSize: 8,
    fontWeight: "600",
  },

  ticketFilterActive: {
    backgroundColor: "rgba(245,111,34,0.12)",
  },

  ticketFilterOptions: {
    gap: 8,
    paddingBottom: 14,
  },

  ticketFilterChip: {
    minHeight: 32,
    borderWidth: 1,
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: 12,
  },

  ticketFilterChipText: {
    fontSize: 10,
    fontWeight: "800",
  },

  ticketList: {
    gap: 20,
  },

  ticketEmptyState: {
    borderRadius: 8,
    padding: 20,
    alignItems: "center",
  },

  ticketEmptyTitle: {
    fontSize: 14,
    fontWeight: "900",
    marginTop: 10,
    marginBottom: 5,
    textAlign: "center",
  },

  ticketEmptyText: {
    fontSize: 11,
    lineHeight: 16,
    textAlign: "center",
  },

  ticketCardCustom: {
    height: 90,
    borderRadius: 12,
    backgroundColor: "#171717",
    flexDirection: "row",
    alignItems: "center",
    overflow: "hidden",
  },

  ticketCardImage: {
    width: 100,
    height: 100,
    resizeMode: "cover",
  },

  ticketCardContent: {
    flex: 1,
    paddingHorizontal: 10,
    paddingVertical: 8,
  },

  ticketCardTop: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 4,
  },

  ticketCardTitle: {
    color: "#FFFFFF",
    fontSize: 12,
    fontWeight: "900",
    flex: 1,
    marginRight: 6,
  },

  ticketCardInfo: {
    color: "#DADADA",
    fontSize: 10,
    marginBottom: 2,
  },
};
