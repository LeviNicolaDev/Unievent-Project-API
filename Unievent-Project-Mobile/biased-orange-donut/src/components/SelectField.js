import { useMemo, useState } from "react";
import {
  Modal,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";

import { BLACK, ORANGE } from "../constants/theme";

export default function SelectField({
  disabled = false,
  modalTitle = "Selecione uma opção",
  onChange,
  options,
  placeholder = "Selecione",
  value,
  variant = "dark",
}) {
  const [visible, setVisible] = useState(false);
  const selectedOption = useMemo(
    () => options.find((option) => String(option.value) === String(value)),
    [options, value]
  );
  const isOrange = variant === "orange";
  const borderColor = isOrange ? ORANGE : "#FFFFFF";
  const textColor = isOrange ? BLACK : "#FFFFFF";
  const modalAccent = isOrange ? ORANGE : BLACK;

  function selectOption(nextValue) {
    onChange(String(nextValue));
    setVisible(false);
  }

  return (
    <>
      <TouchableOpacity
        activeOpacity={0.85}
        disabled={disabled}
        onPress={() => setVisible(true)}
        style={[
          selectStyles.button,
          {
            borderColor,
            opacity: disabled ? 0.6 : 1,
          },
        ]}
      >
        <Text style={[selectStyles.buttonText, { color: textColor }]}>
          {selectedOption?.label || placeholder}
        </Text>
        <Text style={[selectStyles.arrow, { color: textColor }]}>v</Text>
      </TouchableOpacity>

      <Modal visible={visible} transparent animationType="fade">
        <View style={selectStyles.overlay}>
          <View style={selectStyles.sheet}>
            <View style={selectStyles.header}>
              <Text style={selectStyles.title}>{modalTitle}</Text>
              <TouchableOpacity onPress={() => setVisible(false)}>
                <Text style={selectStyles.close}>Fechar</Text>
              </TouchableOpacity>
            </View>

            <ScrollView showsVerticalScrollIndicator={false}>
              {options.map((option) => {
                const active = String(option.value) === String(value);

                return (
                  <TouchableOpacity
                    key={String(option.value)}
                    onPress={() => selectOption(option.value)}
                    style={[
                      selectStyles.option,
                      active ? { borderColor: modalAccent } : null,
                    ]}
                  >
                    <Text
                      style={[
                        selectStyles.optionText,
                        active
                          ? { color: modalAccent, fontWeight: "900" }
                          : null,
                      ]}
                    >
                      {option.label}
                    </Text>
                  </TouchableOpacity>
                );
              })}
            </ScrollView>
          </View>
        </View>
      </Modal>
    </>
  );
}

const selectStyles = StyleSheet.create({
  arrow: {
    fontSize: 18,
    fontWeight: "900",
  },
  button: {
    alignItems: "center",
    borderRadius: 8,
    borderWidth: 2,
    flexDirection: "row",
    height: 52,
    justifyContent: "space-between",
    marginBottom: 14,
    paddingHorizontal: 12,
  },
  buttonText: {
    flex: 1,
    fontSize: 12,
    fontWeight: "800",
    marginRight: 10,
  },
  close: {
    color: ORANGE,
    fontSize: 12,
    fontWeight: "900",
  },
  header: {
    alignItems: "center",
    flexDirection: "row",
    justifyContent: "space-between",
    marginBottom: 10,
  },
  option: {
    borderColor: "#E5E5E5",
    borderRadius: 8,
    borderWidth: 1,
    marginBottom: 8,
    paddingHorizontal: 12,
    paddingVertical: 13,
  },
  optionText: {
    color: BLACK,
    fontSize: 13,
    fontWeight: "700",
  },
  overlay: {
    backgroundColor: "rgba(0,0,0,0.55)",
    flex: 1,
    justifyContent: "flex-end",
  },
  sheet: {
    backgroundColor: "#FFFFFF",
    borderTopLeftRadius: 16,
    borderTopRightRadius: 16,
    maxHeight: "78%",
    padding: 18,
  },
  title: {
    color: BLACK,
    fontSize: 16,
    fontWeight: "900",
  },
});
