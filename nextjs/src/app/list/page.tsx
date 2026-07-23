import { cookies } from "next/headers";
import { getAsync } from "@/services/apiHandler";
import CreateForm from "./create-form";
import UserList from "@/components/UserList";
import { logoutAction } from "@/app/actions";

export const metadata = {
  title: "User Directory",
  description: "Manage system credentials and registered users.",
};





function getUsernameFromToken(token?: string): string {
  if (!token) return "User";
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return "User";
    const payload = JSON.parse(Buffer.from(parts[1], "base64").toString("utf-8"));
    return (
      payload.unique_name ||
      payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
      payload.sub ||
      "User"
    );
  } catch (error) {
    console.error("Error decoding username in list page:", error);
    return "User";
  }
}

export default async function ListPage() {
  const cookieStore = await cookies();
  const token = cookieStore.get("access_token")?.value;
  const loggedInUsername = getUsernameFromToken(token);

  // Fetch users list from backend API
  const response = await getAsync<ApiResponse<UserDto[]>>("api/User/list", true);
  const users = response?.isSuccess && response.data ? response.data : [];

  const imageServerUrl = process.env.IMAGE_SERVER_URL || "https://localhost:7180";

  return (
    <div className="page-container">
      {/* Page Header */}
      <header className="page-header">
        <div className="header-text">
          <h2>User Directory</h2>
          <p>Manage system access credentials and review current registered users.</p>
        </div>
        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          <span style={{ fontSize: "0.95rem", fontWeight: 600, color: "#475569" }}>
            Hello, {loggedInUsername}
          </span>
          <form action={logoutAction}>
            <button type="submit" className="btn-logout">
              Logout
            </button>
          </form>
        </div>
      </header>

      {/* Grid Content */}
      <div className="content-grid">
        {/* Left Column: Create credentials */}
        <div>
          <CreateForm />
        </div>

        {/* Right Column: Registered Users */}
        <div>
          <UserList users={users} imageServerUrl={imageServerUrl} />
        </div>
      </div>
    </div>
  );
}
