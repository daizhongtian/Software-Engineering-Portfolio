import { Fragment, useCallback, useEffect, useState } from "react";
import { getJson, sendJson } from "../api/client";

export default function Screenings() {
  const [screenings, setScreenings] = useState([]);
  const [cinemas, setCinemas] = useState([]);
  const [currentUser, setCurrentUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const [seatMap, setSeatMap] = useState(null);
  const [isSeatsLoading, setIsSeatsLoading] = useState(false);
  const [seatMessage, setSeatMessage] = useState("");

  const [form, setForm] = useState({
    filmTitle: "",
    startTime: "",
    cinemaId: ""
  });

  const isAdmin = currentUser?.roles?.includes("Admin");
  const isAuthenticated = currentUser?.isAuthenticated === true;

  const loadData = useCallback(async () => {
    try {
      setError("");

      const [screeningsData, userData, cinemasData] = await Promise.all([
        getJson("/api/screenings"),
        getJson("/api/account/me"),
        getJson("/api/cinemas")
      ]);

      setScreenings(screeningsData);
      setCurrentUser(userData);
      setCinemas(cinemasData);

      if (cinemasData.length > 0) {
        setForm((current) => ({
          ...current,
          cinemaId: current.cinemaId || String(cinemasData[0].id)
        }));
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    const timeoutId = window.setTimeout(loadData, 0);
    return () => window.clearTimeout(timeoutId);
  }, [loadData]);

  function updateForm(event) {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value
    }));
  }

  function openDateTimePicker(event) {
    event.currentTarget.showPicker?.();
  }

  async function createScreening(event) {
    event.preventDefault();

    if (!form.filmTitle.trim()) {
      setError("Film title is required.");
      return;
    }

    if (!form.startTime) {
      setError("Start time is required.");
      return;
    }

    if (!form.cinemaId) {
      setError("Cinema is required.");
      return;
    }

    try {
      setIsSaving(true);
      setError("");

      await sendJson("/api/screenings", "POST", {
        filmTitle: form.filmTitle,
        startTime: form.startTime,
        cinemaId: Number(form.cinemaId)
      });

      setForm((current) => ({
        ...current,
        filmTitle: "",
        startTime: ""
      }));

      const data = await getJson("/api/screenings");
      setScreenings(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSaving(false);
    }
  }

  async function deleteScreening(id) {
    const confirmed = window.confirm("Delete this screening?");
    if (!confirmed) {
      return;
    }

    try {
      setError("");
      await sendJson(`/api/screenings/${id}`, "DELETE");
      const data = await getJson("/api/screenings");
      setScreenings(data);

      if (seatMap?.screeningId === id) {
        setSeatMap(null);
      }
    } catch (err) {
      setError(err.message);
    }
  }

  async function loadSeats(screeningId, clearMessages = true) {
    setIsSeatsLoading(true);

    if (clearMessages) {
      setError("");
      setSeatMessage("");
    }

    try {
      const data = await getJson(`/api/screenings/${screeningId}/seats`);
      setSeatMap(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSeatsLoading(false);
    }
  }

  async function reserveSeat(seat) {
    try {
      setError("");
      setSeatMessage("");

      await sendJson(`/api/screenings/${seatMap.screeningId}/seats`, "POST", {
        rowNumber: seat.rowNumber,
        seatNumber: seat.seatNumber
      });

      setSeatMessage("Seat reserved.");
      await loadSeats(seatMap.screeningId, false);
    } catch (err) {
      setError(err.message);
      await loadSeats(seatMap.screeningId, false);
    }
  }

  async function cancelSeat(seat) {
    try {
      setError("");
      setSeatMessage("");

      await sendJson(
        `/api/screenings/${seatMap.screeningId}/seats/${seat.rowNumber}/${seat.seatNumber}`,
        "DELETE"
      );

      setSeatMessage("Seat released.");
      await loadSeats(seatMap.screeningId, false);
    } catch (err) {
      setError(err.message);
      await loadSeats(seatMap.screeningId, false);
    }
  }

  function getSeatButtonClass(seat) {
    if (seat.isMine) {
      return "seat-button seat-button-mine";
    }

    if (seat.isReserved) {
      return "seat-button seat-button-reserved";
    }

    return "seat-button seat-button-free";
  }

  if (isLoading) {
    return (
      <div className="container mt-4">
        <p>Loading screenings...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2>Screenings</h2>

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      {isAdmin && (
        <form className="border rounded p-3 mb-4" onSubmit={createScreening}>
          <h3 className="h5 mb-3">Create Screening</h3>

          <div className="row g-3 align-items-end">
            <div className="col-md-4">
              <label className="form-label" htmlFor="filmTitle">
                Film title
              </label>
              <input
                id="filmTitle"
                name="filmTitle"
                className="form-control"
                value={form.filmTitle}
                onChange={updateForm}
              />
            </div>

            <div className="col-md-4">
              <label className="form-label" htmlFor="startTime">
                Start time
              </label>
              <input
                id="startTime"
                name="startTime"
                className="form-control"
                type="datetime-local"
                value={form.startTime}
                onChange={updateForm}
                onClick={openDateTimePicker}
                onFocus={openDateTimePicker}
              />
            </div>

            <div className="col-md-3">
              <label className="form-label" htmlFor="cinemaId">
                Cinema
              </label>
              <select
                id="cinemaId"
                name="cinemaId"
                className="form-select"
                value={form.cinemaId}
                onChange={updateForm}
              >
                {cinemas.map((cinema) => (
                  <option key={cinema.id} value={cinema.id}>
                    {cinema.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="col-md-auto">
              <button
                type="submit"
                className="btn btn-primary px-4"
                disabled={isSaving}
              >
                {isSaving ? "..." : "Create"}
              </button>
            </div>
          </div>
        </form>
      )}

      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Film title</th>
            <th>Start time</th>
            <th>Cinema</th>
            <th>Seats</th>
            {isAdmin && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {screenings.map((screening) => (
            <tr key={screening.id}>
              <td>{screening.filmTitle}</td>
              <td>{new Date(screening.startTime).toLocaleString()}</td>
              <td>{screening.cinemaName}</td>
              <td>
                <button
                  type="button"
                  className="btn btn-sm btn-outline-primary"
                  onClick={() => loadSeats(screening.id)}
                >
                  View seats
                </button>
              </td>
              {isAdmin && (
                <td>
                  <button
                    type="button"
                    className="btn btn-sm btn-danger"
                    onClick={() => deleteScreening(screening.id)}
                  >
                    Delete
                  </button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>

      {seatMap && (
        <div className="seat-card mt-4">
          <div className="seat-card-header">
            <h3>{seatMap.filmTitle}</h3>
            <p>
              {new Date(seatMap.startTime).toLocaleString()} - {seatMap.cinemaName}
            </p>
          </div>

          {seatMessage && (
            <div className="alert alert-success">
              {seatMessage}
            </div>
          )}

          {isSeatsLoading ? (
            <p>Loading seats...</p>
          ) : (
            <>
              <div className="seat-map-wrapper">
                <div
                  className="seat-map"
                  style={{ gridTemplateColumns: `3.4rem repeat(${seatMap.seatsPerRow}, 3.9rem)` }}
                >
                  <div className="seat-grid-corner" />

                  {Array.from({ length: seatMap.seatsPerRow }, (_, seatIndex) => (
                    <div key={`column-${seatIndex + 1}`} className="seat-column-label">
                      {seatIndex + 1}
                    </div>
                  ))}

                  {Array.from({ length: seatMap.rows }, (_, rowIndex) => {
                    const rowNumber = rowIndex + 1;

                    return (
                      <Fragment key={rowNumber}>
                        <div className="seat-row-label">R{rowNumber}</div>

                        {Array.from({ length: seatMap.seatsPerRow }, (_, seatIndex) => {
                          const seatNumber = seatIndex + 1;
                          const seat = seatMap.seats.find((candidate) =>
                            candidate.rowNumber === rowNumber &&
                            candidate.seatNumber === seatNumber
                          );

                          if (!seat) {
                            return null;
                          }

                          return (
                            <button
                              key={`${seat.rowNumber}-${seat.seatNumber}`}
                              type="button"
                              className={getSeatButtonClass(seat)}
                              disabled={!isAuthenticated || (seat.isReserved && !seat.isMine)}
                              onClick={() => {
                                if (seat.isMine) {
                                  cancelSeat(seat);
                                } else {
                                  reserveSeat(seat);
                                }
                              }}
                            >
                              {seat.seatNumber}
                            </button>
                          );
                        })}
                      </Fragment>
                    );
                  })}
                </div>
              </div>

              <div className="seat-legend">
                <span>
                  <span className="seat-legend-box seat-legend-free" />
                  Available
                </span>
                <span>
                  <span className="seat-legend-box seat-legend-reserved" />
                  Reserved
                </span>
                <span>
                  <span className="seat-legend-box seat-legend-mine" />
                  My seat
                </span>
              </div>
            </>
          )}
        </div>
      )}
    </div>
  );
}
