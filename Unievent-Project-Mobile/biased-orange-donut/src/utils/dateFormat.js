export function formatDateForInput(value) {
  if (!value) return "";

  const text = String(value);
  const isoDate = text.match(/^(\d{4})-(\d{2})-(\d{2})/);
  if (isoDate) {
    return `${isoDate[3]}/${isoDate[2]}/${isoDate[1]}`;
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";

  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const year = date.getFullYear();

  return `${day}/${month}/${year}`;
}

export function normalizeBrazilianDateInput(value) {
  const trimmed = String(value || "").trim();
  const digitsDate = trimmed.match(/^(\d{2})(\d{2})(\d{4})$/);
  const brDate = trimmed.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
  const match = brDate || digitsDate;

  if (!match) {
    throw new Error("Informe a data no formato dd/mm/aaaa.");
  }

  const day = Number(match[1]);
  const month = Number(match[2]);
  const year = Number(match[3]);
  const date = new Date(Date.UTC(year, month - 1, day));

  if (
    date.getUTCFullYear() !== year ||
    date.getUTCMonth() !== month - 1 ||
    date.getUTCDate() !== day
  ) {
    throw new Error("Informe uma data de nascimento válida.");
  }

  const today = new Date();
  const todayUtc = new Date(
    Date.UTC(today.getFullYear(), today.getMonth(), today.getDate())
  );
  if (date >= todayUtc) {
    throw new Error("A data de nascimento deve ser anterior à data atual.");
  }

  return `${year}-${String(month).padStart(2, "0")}-${String(day).padStart(
    2,
    "0"
  )}`;
}
