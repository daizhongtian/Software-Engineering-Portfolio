import { useState } from "react";
import { sendJson } from "../api/client";

export default function Register({ onRegistered }) {
  const [form, setForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
    phoneNumber: "",
    password: "",
    confirmPassword: ""
  });
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  function updateForm(event) {
    const { name, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: value
    }));
  }

  async function register(event) {
    event.preventDefault();

    try {
      setIsSaving(true);
      setError("");
      setMessage("");

      await sendJson("/api/account/register", "POST", form);
      await onRegistered?.();

      setMessage("Account created. You are now signed in.");
      setForm({
        email: "",
        firstName: "",
        lastName: "",
        phoneNumber: "",
        password: "",
        confirmPassword: ""
      });
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div className="container mt-4">
      <h2>Register</h2>

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

      <form className="border rounded p-3" onSubmit={register}>
        <div className="mb-3">
          <label className="form-label" htmlFor="registerEmail">
            Email
          </label>
          <input
            id="registerEmail"
            name="email"
            className="form-control"
            value={form.email}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="registerFirstName">
            First name
          </label>
          <input
            id="registerFirstName"
            name="firstName"
            className="form-control"
            value={form.firstName}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="registerLastName">
            Last name
          </label>
          <input
            id="registerLastName"
            name="lastName"
            className="form-control"
            value={form.lastName}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="registerPhoneNumber">
            Phone number
          </label>
          <input
            id="registerPhoneNumber"
            name="phoneNumber"
            className="form-control"
            value={form.phoneNumber}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="registerPassword">
            Password
          </label>
          <input
            id="registerPassword"
            name="password"
            type="password"
            className="form-control"
            value={form.password}
            onChange={updateForm}
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="registerConfirmPassword">
            Confirm password
          </label>
          <input
            id="registerConfirmPassword"
            name="confirmPassword"
            type="password"
            className="form-control"
            value={form.confirmPassword}
            onChange={updateForm}
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary"
          disabled={isSaving}
        >
          {isSaving ? "Creating..." : "Register"}
        </button>
      </form>
    </div>
  );
}
