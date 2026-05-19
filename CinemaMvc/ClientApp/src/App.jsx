import { useState } from "react";
import Screenings from "./pages/Screenings";
import ProfileEdit from "./pages/ProfileEdit";
import AdminUsers from "./pages/AdminUsers";
import Register from "./pages/Register";

export default function App() {
  const [page, setPage] = useState("screenings");

  return (
    <>
      <nav className="navbar navbar-expand navbar-light bg-light border-bottom">
        <div className="container">
          <span className="navbar-brand">CinemaMvc</span>

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

            <button
              type="button"
              className="nav-link btn btn-link"
              onClick={() => setPage("users")}
            >
              Users
            </button>

            <button
              type="button"
              className="nav-link btn btn-link"
              onClick={() => setPage("register")}
            >
              Register
            </button>
          </div>
        </div>
      </nav>

      {page === "screenings" && <Screenings />}
      {page === "profile" && <ProfileEdit />}
      {page === "users" && <AdminUsers />}
      {page === "register" && <Register />}
    </>
  );
}
