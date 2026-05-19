import { useEffect, useState } from "react";
import { getJson, sendJson } from "../api/client";

export default function Screenings() {
  const [screenings, setScreenings] = useState([]);
  const [cinemas, setCinemas] = useState([]);
  const [currentUser, setCurrentUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const [form, setForm] = useState({
    filmTitle: "",
    startTime: "",
    cinemaId: ""
  });

  const isAdmin = currentUser?.roles?.includes("Admin");

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
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
  }

  function updateForm(event) {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value
    }));
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
    } catch (err) {
      setError(err.message);
    }
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

            <div className="col-md-1">
              <button
                type="submit"
                className="btn btn-primary w-100"
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
            {isAdmin && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {screenings.map((screening) => (
            <tr key={screening.id}>
              <td>{screening.filmTitle}</td>
              <td>{new Date(screening.startTime).toLocaleString()}</td>
              <td>{screening.cinemaName}</td>
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
    </div>
  );
}
