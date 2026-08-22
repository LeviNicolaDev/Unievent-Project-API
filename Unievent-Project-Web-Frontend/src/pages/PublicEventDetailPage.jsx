import { CalendarDays, MapPin, Ticket, UsersRound } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getEventById } from "../services/eventService.js";
import { formatDate, getAssetUrl } from "../utils/formatters.js";

const AUDIENCE_LABELS = {
  PublicoGeral: "Público geral",
  publicoGeral: "Público geral",
  TodosAlunosFatec: "Todos os alunos FATEC",
  todosAlunosFatec: "Todos os alunos FATEC",
  AlunosDaInstituicao: "Somente alunos desta instituição",
  alunosDaInstituicao: "Somente alunos desta instituição",
};

function getAudienceLabel(event) {
  return (
    AUDIENCE_LABELS[event.audience || event.publicoPermitido] || "Público geral"
  );
}

export function PublicEventDetailPage() {
  const { id } = useParams();
  const [event, setEvent] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadEvent() {
      setIsLoading(true);
      setError(null);
      try {
        setEvent(await getEventById(id, { skipAuth: true }));
      } catch (err) {
        setError(err.message || "Evento não encontrado");
      } finally {
        setIsLoading(false);
      }
    }

    loadEvent();
  }, [id]);

  if (isLoading) {
    return (
      <main className="public-event-detail">
        <p>Carregando evento...</p>
      </main>
    );
  }

  if (error || !event) {
    return (
      <main className="public-event-detail">
        <Link to="/descobrir-eventos">Voltar</Link>
        <p>{error || "Evento não encontrado"}</p>
      </main>
    );
  }

  return (
    <main className="public-event-detail">
      <Link to="/descobrir-eventos">Voltar</Link>
      <section>
        <img
          src={event.image || getAssetUrl("evento.png")}
          alt=""
          onError={(img) => {
            img.currentTarget.src = getAssetUrl("evento.png");
          }}
        />
        <div>
          <span>{event.instituicaoNome || "Instituição não informada"}</span>
          <h1>{event.nome}</h1>
          <p>{event.descricao}</p>
          <ul>
            <li>
              <CalendarDays size={18} /> {formatDate(event.dataEvento, "pt")}{" "}
              {event.time}
            </li>
            <li>
              <Ticket size={18} /> Público: {getAudienceLabel(event)}
            </li>
            <li>
              <UsersRound size={18} /> {event.capacidade} vagas
            </li>
            {event.local && (
              <li>
                <MapPin size={18} /> {event.local}
              </li>
            )}
            {(event.cidade || event.estado) && (
              <li>
                <MapPin size={18} />{" "}
                {[event.cidade, event.estado].filter(Boolean).join(" - ")}
              </li>
            )}
          </ul>
        </div>
      </section>
    </main>
  );
}
