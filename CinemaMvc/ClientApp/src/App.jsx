import { useCallback, useEffect, useRef, useState } from "react";
import Screenings from "./pages/Screenings";
import ProfileEdit from "./pages/ProfileEdit";
import AdminUsers from "./pages/AdminUsers";
import Register from "./pages/Register";
import { getJson, sendJson } from "./api/client";

const identityBaseUrl = import.meta.env.DEV ? "http://localhost:5239" : "";
const loginReturnUrl = encodeURIComponent(import.meta.env.DEV ? window.location.origin : "/");
const anonymousUser = {
  isAuthenticated: false,
  email: null,
  roles: []
};

function getAuthKey(user) {
  return `${user.isAuthenticated}:${user.email ?? ""}:${(user.roles ?? []).join(",")}`;
}

async function loadCurrentUser() {
  try {
    return await getJson("/api/account/me");
  } catch {
    return anonymousUser;
  }
}

export default function App() {
  const [page, setPage] = useState("screenings");
  const [currentUser, setCurrentUser] = useState(anonymousUser);
  const [authVersion, setAuthVersion] = useState(0);
  const authKeyRef = useRef(getAuthKey(anonymousUser));

  const isAdmin = currentUser.roles?.includes("Admin");
  const visiblePage = page === "users" && !isAdmin ? "screenings" : page;

  const applyCurrentUser = useCallback((nextUser) => {
    const nextAuthKey = getAuthKey(nextUser);
    if (nextAuthKey !== authKeyRef.current) {
      authKeyRef.current = nextAuthKey;
      setAuthVersion((current) => current + 1);
    }

    setCurrentUser(nextUser);
  }, []);

  const refreshCurrentUser = useCallback(async () => {
    applyCurrentUser(await loadCurrentUser());
  }, [applyCurrentUser]);

  async function logout() {
    await sendJson("/api/account/logout", "POST");
    setPage("screenings");
    await refreshCurrentUser();
  }

  useEffect(() => {
    let isMounted = true;

    loadCurrentUser().then((nextUser) => {
      if (isMounted) {
        applyCurrentUser(nextUser);
      }
    });

    return () => {
      isMounted = false;
    };
  }, [applyCurrentUser]);

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

            {currentUser.isAuthenticated && (
              <button
                type="button"
                className="nav-link btn btn-link"
                onClick={() => setPage("profile")}
              >
                My Profile
              </button>
            )}

            {isAdmin && (
              <button
                type="button"
                className="nav-link btn btn-link"
                onClick={() => setPage("users")}
              >
                Users
              </button>
            )}

            {!currentUser.isAuthenticated && (
              <button
                type="button"
                className="nav-link btn btn-link"
                onClick={() => setPage("register")}
              >
                Register
              </button>
            )}

            {!currentUser.isAuthenticated && (
              <a
                className="nav-link"
                href={`${identityBaseUrl}/Identity/Account/Login?returnUrl=${loginReturnUrl}`}
              >
                Login
              </a>
            )}

            {currentUser.isAuthenticated && (
              <button
                type="button"
                className="nav-link btn btn-link"
                onClick={logout}
              >
                Logout
              </button>
            )}
          </div>

          <span className="navbar-text small ms-auto">
            {currentUser.isAuthenticated
              ? `${currentUser.email}${isAdmin ? " (Admin)" : ""}`
              : "Not signed in"}
          </span>
        </div>
      </nav>

      {visiblePage === "screenings" && <Screenings key={`screenings-${authVersion}`} />}
      {visiblePage === "profile" && <ProfileEdit key={`profile-${authVersion}`} />}
      {visiblePage === "users" && isAdmin && <AdminUsers key={`users-${authVersion}`} />}
      {visiblePage === "register" && <Register onRegistered={refreshCurrentUser} />}
    </>
  );
}
