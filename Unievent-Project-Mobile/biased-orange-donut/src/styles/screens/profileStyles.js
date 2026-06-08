import { ORANGE } from "../../constants/theme";

export const profileStyles = {
  // PROFILE
  profileScroll: {
    paddingTop: 42,
    paddingBottom: 120,
  },

  profileHeader: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 46,
  },

  profileHeaderTitle: {
    fontSize: 17,
    fontWeight: "900",
  },

  profileHeaderIcons: {
    flexDirection: "row",
    alignItems: "center",
    gap: 14,
  },

  profileMain: {
    alignItems: "center",
    marginBottom: 28,
  },

  avatarCircle: {
    width: 112,
    height: 112,
    borderRadius: 56,
    backgroundColor: "#00BFA6",
    alignItems: "center",
    justifyContent: "center",
    borderWidth: 4,
    borderColor: "#242424",
  },

  avatarEmoji: {
    fontSize: 52,
  },

  avatarImage: {
    width: 112,
    height: 112,
    borderRadius: 56,
    borderWidth: 4,
    borderColor: "#242424",
    backgroundColor: "#1F1F1F",
  },

  profileName: {
    fontSize: 16,
    fontWeight: "900",
    marginTop: 12,
  },

  profileEmail: {
    fontSize: 11,
    marginTop: 4,
  },

  profileBtns: {
    flexDirection: "row",
    flexWrap: "wrap",
    justifyContent: "center",
    gap: 10,
    marginTop: 18,
  },

  profileButton: {
    paddingHorizontal: 15,
    paddingVertical: 9,
    borderRadius: 5,
  },

  profileButtonText: {
    color: "#FFFFFF",
    fontSize: 10,
    fontWeight: "900",
  },

  profileStatsRow: {
    flexDirection: "row",
    gap: 10,
    marginBottom: 28,
  },

  profileStatBox: {
    flex: 1,
    borderRadius: 10,
    paddingVertical: 13,
    alignItems: "center",
  },

  profileStatNumber: {
    fontSize: 18,
    fontWeight: "900",
  },

  profileStatLabel: {
    fontSize: 9,
    marginTop: 3,
  },

  profileSectionTitle: {
    fontSize: 15,
    fontWeight: "900",
    marginBottom: 12,
  },

  profileTags: {
    flexDirection: "row",
    flexWrap: "wrap",
    gap: 8,
  },

  profileTag: {
    borderWidth: 1,
    borderRadius: 6,
    paddingHorizontal: 12,
    paddingVertical: 7,
  },

  profileTagText: {
    fontSize: 10,
    fontWeight: "600",
  },

  // MODAL
  modalOverlay: {
    flex: 1,
    backgroundColor: "rgba(0,0,0,0.55)",
    alignItems: "center",
    justifyContent: "center",
    paddingHorizontal: 22,
  },

  editModal: {
    width: "100%",
    maxHeight: "88%",
    borderRadius: 18,
    padding: 20,
  },

  modalHeader: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 18,
  },

  modalTitle: {
    fontSize: 20,
    fontWeight: "900",
  },

  profileInputGroup: {
    marginBottom: 13,
  },

  profileInputLabel: {
    fontSize: 12,
    fontWeight: "700",
    marginBottom: 6,
  },

  profileInputBox: {
    height: 48,
    borderWidth: 1.5,
    borderRadius: 8,
    paddingHorizontal: 12,
    flexDirection: "row",
    alignItems: "center",
  },

  profileInput: {
    flex: 1,
    marginLeft: 10,
    fontSize: 12,
  },

  profilePhotoEditRow: {
    flexDirection: "row",
    alignItems: "center",
    gap: 12,
    marginBottom: 14,
  },

  profilePhotoPreview: {
    width: 58,
    height: 58,
    borderRadius: 29,
    backgroundColor: "#1F1F1F",
  },

  profilePhotoPreviewFallback: {
    width: 58,
    height: 58,
    borderRadius: 29,
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "#2A2A2A",
  },

  profilePhotoButton: {
    flex: 1,
    height: 44,
    borderWidth: 1.5,
    borderRadius: 8,
    paddingHorizontal: 12,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    gap: 8,
  },

  profilePhotoButtonText: {
    flexShrink: 1,
    fontSize: 12,
    fontWeight: "800",
  },

  profileModalError: {
    fontSize: 12,
    fontWeight: "700",
    lineHeight: 16,
    marginTop: 2,
    marginBottom: 8,
    textAlign: "center",
  },

  modalSaveButton: {
    height: 48,
    borderRadius: 8,
    alignItems: "center",
    justifyContent: "center",
    marginTop: 8,
  },

  modalSaveButtonText: {
    color: "#FFFFFF",
    fontSize: 14,
    fontWeight: "900",
  },
};
