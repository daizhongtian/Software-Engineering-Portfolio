import { useEffect, useState } from "react";
import Screenings from "./pages/Screenings";
import ProfileEdit from "./pages/ProfileEdit";
import AdminUsers from "./pages/AdminUsers";
import Register from "./pages/Register";
import { getJson } from "./api/client";

const identityBaseUrl = import.meta.env.DEV ? "http://localhost:5239" : "";

export default function App() {
  const [page, setPage] = useState("screenings");
  const [currentUser, setCurrentUser] = useState({
    isAuthenticated: false,
    email: null,
    roles: []
  });
  const [authVersion, setAuthVersion] = useState(0);

  const isAdmin = currentUser.roles?.includes("Admin");

  useEffect(() => {
    refreshCurrentUser();

    window.addEventListener("focus", refreshCurrentUser);
    return () => window.removeEventListener("focus", refreshCurrentUser);
  }, []);

  useEffect(() => {
    if (page === "users" && !isAdmin) {
      setPage("screenings");
    }
  }, [isAdmin, page]);

  async function refreshCurrentUser() {
    try {
      const data = await getJson("/api/account/me");
      setCurrentUser(data);
    } catch {
      setCurrentUser({
        isAuthenticated: false,
        email: null,
        roles: []
      });
    } finally {
      setAuthVersion((current) => current + 1);
    }
  }

  return (
    <>
      <nav className="navbar navbar-expand navbar-light bg-light border-bottom">
        <div className="container gap-3">
          <button
            type="button"
            className="navbar-brand btn btn-link p-0 text-decoration-none"
            onClick={() => setPage("screenings")}
          >
            CinemaMvc
          </button>

          <div className="navbar-nav">
            <button
              type="button"
              className="nav-link btn btn-link"
              onClick={() => setPage("screenings")}
            >
              Screenings
            </button>

            <button
              type="button"
              className="nav-link btn btn-link"
              onClick={() => setPage("profile")}
            >
              My Profile
            </button>

            {isAdmin && (
              <button
                type="button"
                className="nav-link btn btn-link"
                onClick={() => setPage("users")}
              >
                Users
              </button>
            )}

            <button
              type="button"
              className="nav-link btn btn-link"
              onClick={() => setPage("register")}
            >
              Register
            </button>

            {!currentUser.isAuthenticated && (
              <a
                className="nav-link"
                href={`${identityBaseUrl}/Identity/Account/Login`}
              >
                Login
              </a>
            )}
          </div>

          <span className="navbar-text small ms-auto">
            {currentUser.isAuthenticated
              ? `${currentUser.email}${isAdmin ? " (Admin)" : ""}`
              : "Not signed in"}
          </span>
        </div>
      </nav>

      {page === "screenings" && <Screenings key={`screenings-${authVersion}`} />}
      {page === "profile" && <ProfileEdit key={`profile-${authVersion}`} />}
      {page === "users" && isAdmin && <AdminUsers key={`users-${authVersion}`} />}
      {page === "register" && <Register />}
    </>
  );
}
