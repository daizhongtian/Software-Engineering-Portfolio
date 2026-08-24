import { useEffect, useState } from "react";
import { getJson, sendJson } from "../api/client";

export default function ProfileEdit() {
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    phoneNumber: "",
    concurrencyStamp: ""
  });
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    loadProfile();
  }, []);

  async function loadProfile() {
    try {
      setError("");
      const data = await getJson("/api/profile");

      setForm({
        firstName: data.firstName ?? "",
        lastName: data.lastName ?? "",
        phoneNumber: data.phoneNumber ?? "",
        concurrencyStamp: data.concurrencyStamp ?? ""
      });
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

  async function saveProfile(event) {
    event.preventDefault();

    if (!form.firstName.trim()) {
      setError("First name is required.");
      return;
    }

    if (!form.lastName.trim()) {
      setError("Last name is required.");
      return;
    }

    try {
      setIsSaving(true);
      setError("");
      setMessage("");

      await sendJson("/api/profile", "PUT", form);

      setMessage("Profile updated.");
      await loadProfile();
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSaving(false);
    }
  }

  if (isLoading) {
    return (
      <div className="container mt-4">
        <p>Loading profile...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2>My Profile</h2>

      {message && (
        <div className="alert alert-success">
          {message}
        </div>
      )}

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      <form className="border rounded p-3" onSubmit={saveProfile}>
        <div className="mb-3">
          <label className="form-label" htmlFor="profileFirstName">
            First name
          </label>
          <input
            id="profileFirstName"
            name="firstName"
            className="form-control"
            value={form.firstName}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="profileLastName">
            Last name
          </label>
          <input
            id="profileLastName"
            name="lastName"
            className="form-control"
            value={form.lastName}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="profilePhoneNumber">
            Phone number
          </label>
          <input
            id="profilePhoneNumber"
            name="phoneNumber"
            className="form-control"
            value={form.phoneNumber}
            onChange={updateForm}
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary"
          disabled={isSaving}
        >
          {isSaving ? "Saving..." : "Save"}
        </button>
      </form>
    </div>
  );
}
