import { useEffect, useState } from "react";
import { getJson, sendJson } from "../api/client";

export default function AdminUsers() {
  const [users, setUsers] = useState([]);
  const [editingUser, setEditingUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    loadUsers();
  }, []);

  async function loadUsers() {
    try {
      setError("");
      const data = await getJson("/api/admin/users");
      setUsers(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsLoading(false);
    }
  }

  function startEdit(user) {
    setMessage("");
    setError("");
    setEditingUser({
      id: user.id,
      email: user.email ?? "",
      firstName: user.firstName ?? "",
      lastName: user.lastName ?? "",
      phoneNumber: user.phoneNumber ?? "",
      concurrencyStamp: user.concurrencyStamp ?? ""
    });
  }

  function cancelEdit() {
    setEditingUser(null);
  }

  function updateEditingUser(event) {
    const { name, value } = event.target;

    setEditingUser((current) => ({
      ...current,
      [name]: value
    }));
  }

  async function saveUser(event) {
    event.preventDefault();

    if (!editingUser.firstName.trim()) {
      setError("First name is required.");
      return;
    }

    if (!editingUser.lastName.trim()) {
      setError("Last name is required.");
      return;
    }

    try {
      setIsSaving(true);
      setError("");
      setMessage("");

      await sendJson(`/api/admin/users/${editingUser.id}`, "PUT", {
        firstName: editingUser.firstName,
        lastName: editingUser.lastName,
        phoneNumber: editingUser.phoneNumber,
        concurrencyStamp: editingUser.concurrencyStamp
      });

      setEditingUser(null);
      setMessage("User updated.");
      await loadUsers();
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSaving(false);
    }
  }

  async function deleteUser(user) {
    const confirmed = window.confirm(`Delete user ${user.email}?`);
    if (!confirmed) {
      return;
    }

    try {
      setError("");
      setMessage("");

      await sendJson(`/api/admin/users/${user.id}`, "DELETE", {
        concurrencyStamp: user.concurrencyStamp
      });

      setMessage("User deleted.");
      await loadUsers();
    } catch (err) {
      setError(err.message);
    }
  }

  if (isLoading) {
    return (
      <div className="container mt-4">
        <p>Loading users...</p>
      </div>
    );
  }

  return (
    <div className="container mt-4">
      <h2>Users</h2>

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

      {editingUser && (
        <form className="border rounded p-3 mb-4" onSubmit={saveUser}>
          <h3 className="h5 mb-3">Edit User</h3>

          <div className="mb-3">
            <label className="form-label" htmlFor="editEmail">
              Email
            </label>
            <input
              id="editEmail"
              className="form-control"
              value={editingUser.email}
              disabled
            />
          </div>

          <div className="row g-3">
            <div className="col-md-4">
              <label className="form-label" htmlFor="editFirstName">
                First name
              </label>
              <input
                id="editFirstName"
                name="firstName"
                className="form-control"
                value={editingUser.firstName}
                onChange={updateEditingUser}
              />
            </div>

            <div className="col-md-4">
              <label className="form-label" htmlFor="editLastName">
                Last name
              </label>
              <input
                id="editLastName"
                name="lastName"
                className="form-control"
                value={editingUser.lastName}
                onChange={updateEditingUser}
              />
            </div>

            <div className="col-md-4">
              <label className="form-label" htmlFor="editPhoneNumber">
                Phone number
              </label>
              <input
                id="editPhoneNumber"
                name="phoneNumber"
                className="form-control"
                value={editingUser.phoneNumber}
                onChange={updateEditingUser}
              />
            </div>
          </div>

          <div className="mt-3">
            <button
              type="submit"
              className="btn btn-primary me-2"
              disabled={isSaving}
            >
              {isSaving ? "Saving..." : "Save"}
            </button>

            <button
              type="button"
              className="btn btn-secondary"
              onClick={cancelEdit}
              disabled={isSaving}
            >
              Cancel
            </button>
          </div>
        </form>
      )}

      <table className="table table-bordered">
        <thead>
          <tr>
            <th>Email</th>
            <th>First name</th>
            <th>Last name</th>
            <th>Phone</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.email}</td>
              <td>{user.firstName}</td>
              <td>{user.lastName}</td>
              <td>{user.phoneNumber}</td>
              <td>
                <button
                  type="button"
                  className="btn btn-sm btn-warning me-2"
                  onClick={() => startEdit(user)}
                >
                  Edit
                </button>

                <button
                  type="button"
                  className="btn btn-sm btn-danger"
                  onClick={() => deleteUser(user)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
