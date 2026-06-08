import { useEffect, useMemo, useState } from "react";
import { useLanguage } from "../../hooks/useLanguage.js";
import { getAssetUrl } from "../../utils/formatters.js";
import { Button } from "../ui/Button.jsx";
import { FormField } from "./FormField.jsx";

const initialForm = {
  title: "",
  description: "",
  capacity: "",
  responsible: "",
  category: "Palestra",
  date: "",
  time: "",
};

const uploadPlaceholder = getAssetUrl("upload.png");
const categoryLabels = {
  Palestra: "categoryLecture",
  Workshop: "categoryWorkshop",
};

function normalizeText(value) {
  return String(value || "")
    .trim()
    .toLowerCase();
}

function getResponsibleName(person) {
  return person.nome || person.Nome || person.name || person.nomeUsuario || "";
}

function findResponsibleId(event, responsiblePeople) {
  const id =
    event?.responsavelEventoId ||
    event?.ResponsavelEventoId ||
    event?.idResponsavelEvento ||
    event?.IdResponsavelEvento ||
    event?.responsibleId ||
    event?.responsible;

  if (id && responsiblePeople.some((person) => String(person.id) === String(id))) {
    return String(id);
  }

  const name =
    event?.responsavel ||
    event?.Responsavel ||
    event?.responsibleName ||
    event?.nomeResponsavel;

  if (!name) {
    return "";
  }

  const match = responsiblePeople.find(
    (person) => normalizeText(getResponsibleName(person)) === normalizeText(name),
  );

  return match ? String(match.id) : "";
}

export function EventForm({ event, responsiblePeople, submitLabel, onSubmit }) {
  const { t } = useLanguage();
  const [form, setForm] = useState(() => ({
    ...initialForm,
    ...event,
    responsible: findResponsibleId(event, responsiblePeople) || event?.responsible || "",
  }));
  const [preview, setPreview] = useState(event?.image || uploadPlaceholder);
  const [imageFile, setImageFile] = useState(null);
  const isUploadPlaceholder = preview === uploadPlaceholder;
  const categories = useMemo(
    () => [
      { value: "Palestra", label: t("categoryLecture") },
      { value: "Workshop", label: t("categoryWorkshop") },
      { value: "Feira", label: "Feira" },
      { value: "Evento", label: "Evento" },
      { value: "Festa", label: "Festa" },
    ],
    [t],
  );

  useEffect(() => {
    setForm((current) => ({
      ...initialForm,
      ...event,
      responsible: findResponsibleId(event, responsiblePeople) || current.responsible || "",
    }));
    setPreview(event?.image || uploadPlaceholder);
    setImageFile(null);
  }, [event, responsiblePeople]);

  function updateField(name, value) {
    setForm((current) => ({ ...current, [name]: value }));
  }

  function handleImage(event) {
    const file = event.target.files?.[0];
    if (file) {
      setImageFile(file);
      setPreview(URL.createObjectURL(file));
    }
  }

  function handleSubmit(event) {
    event.preventDefault();
    onSubmit({ ...form, imageFile });
  }

  function getCategoryLabel(category) {
    const translationKey = categoryLabels[category.value];
    return translationKey ? t(translationKey) : category.label;
  }

  return (
    <form className="event-form" onSubmit={handleSubmit}>
      <div className="form-grid">
        <div className="form-column">
          <FormField label={t("title")}>
            <input
              value={form.title}
              onChange={(event) => updateField("title", event.target.value)}
              required
            />
          </FormField>
          <FormField label={t("description")}>
            <textarea
              value={form.description}
              onChange={(event) =>
                updateField("description", event.target.value)
              }
              required
            />
          </FormField>
          <FormField label={t("capacity")}>
            <input
              type="number"
              value={form.capacity}
              placeholder={t("maxPeople")}
              onChange={(event) => updateField("capacity", event.target.value)}
              min="1"
              required
            />
          </FormField>
        </div>

        <div className="form-column">
          <FormField label={t("responsible")}>
            <select
              value={form.responsible}
              onChange={(event) =>
                updateField("responsible", event.target.value)
              }
              required
            >
              <option value="">{t("selectResponsible")}</option>
              {responsiblePeople.map((person) => (
                <option key={person.id} value={person.id}>
                  {getResponsibleName(person)}
                </option>
              ))}
            </select>
          </FormField>
          <FormField label={t("image")}>
            <span
              className={`upload-box ${isUploadPlaceholder ? "upload-box--placeholder" : ""}`.trim()}
            >
              <input type="file" accept="image/*" onChange={handleImage} required={!event?.id} />
              <img
                src={preview}
                alt={t("preview")}
                onError={() => setPreview(uploadPlaceholder)}
              />
            </span>
          </FormField>
          <div className="inline-fields">
            <FormField label={t("date")}>
              <input
                type="date"
                value={form.date}
                onChange={(event) => updateField("date", event.target.value)}
                required
              />
            </FormField>
            <FormField label={t("time")}>
              <input
                type="time"
                value={form.time}
                onChange={(event) => updateField("time", event.target.value)}
                required
              />
            </FormField>
          </div>
          <FormField label={t("eventType")}>
            <select
              value={form.category}
              onChange={(event) => updateField("category", event.target.value)}
            >
              {categories.map((category) => (
                <option key={category.value} value={category.value}>
                  {getCategoryLabel(category)}
                </option>
              ))}
            </select>
          </FormField>
        </div>
      </div>
      <div className="form-actions">
        <Button type="submit">{submitLabel}</Button>
      </div>
    </form>
  );
}
