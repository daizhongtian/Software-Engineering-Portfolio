import { useState } from "react";
import { sendJson } from "../api/client";

export default function Login({ onLoggedIn }) {
  const [form, setForm] = useState({
    email: "",
    password: "",
    rememberMe: false
  });
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  function updateForm(event) {
    const { name, type, checked, value } = event.target;

    setForm((current) => ({
      ...current,
      [name]: type === "checkbox" ? checked : value
    }));
  }

  async function login(event) {
    event.preventDefault();

    if (!form.email.trim()) {
      setError("Email is required.");
      return;
    }

    if (!form.password) {
      setError("Password is required.");
      return;
    }

    try {
      setIsSaving(true);
      setError("");

      const currentUser = await sendJson("/api/account/login", "POST", form);
      await onLoggedIn?.(currentUser);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <div className="container mt-4">
      <h2>Login</h2>

      {error && (
        <div className="alert alert-danger">
          {error}
        </div>
      )}

      <form className="border rounded p-3" onSubmit={login}>
        <div className="mb-3">
          <label className="form-label" htmlFor="loginEmail">
            Email
          </label>
          <input
            id="loginEmail"
            name="email"
            className="form-control"
            value={form.email}
            onChange={updateForm}
            autoComplete="username"
          />
        </div>

        <div className="mb-3">
          <label className="form-label" htmlFor="loginPassword">
            Password
          </label>
          <input
            id="loginPassword"
            name="password"
            type="password"
            className="form-control"
            value={form.password}
            onChange={updateForm}
            autoComplete="current-password"
          />
        </div>

        <div className="form-check mb-3">
          <input
            id="loginRememberMe"
            name="rememberMe"
            type="checkbox"
            className="form-check-input"
            checked={form.rememberMe}
            onChange={updateForm}
          />
          <label className="form-check-label" htmlFor="loginRememberMe">
            Remember me
          </label>
        </div>

        <button
          type="submit"
          className="btn btn-primary"
          disabled={isSaving}
        >
          {isSaving ? "Signing in..." : "Login"}
        </button>
      </form>
    </div>
  );
}
