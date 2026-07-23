import React from "react";
import BaseListing, { Column, Action } from "./BaseListing";

interface User {
  id: string;
  username: string;
  imageUrl?: string | null;
}

interface UserListProps {
  users: User[];
  imageServerUrl: string;
}

export default function UserList({ users, imageServerUrl }: UserListProps) {
  // Define columns mapping for the User directory using BaseListing structure
  const columns: Column<User>[] = [
    {
      header: "Avatar",
      width: "80px",
      render: (u) => (
        <div className="user-avatar">
          {u.imageUrl ? (
            <img
              src={`${imageServerUrl}${u.imageUrl}`}
              alt="Avatar"
              className="user-avatar-img"
            />
          ) : (
            u.username.substring(0, Math.min(2, u.username.length)).toUpperCase()
          )}
        </div>
      ),
    },
    {
      header: "Username",
      key: "username",
      render: (u) => <span className="username-cell">{u.username}</span>,
    },
    {
      header: "Account Reference ID",
      render: (u) => <span className="user-badge">{u.id.substring(0, 8)}...</span>,
    },
  ];

  // Define action handlers (Edit, Delete) matching BaseListing functionality
  const actions: Action<User>[] = [
    {
      label: "Edit",
      className: "btn-action btn-action-primary",
      onClick: (u) => {
        alert(`Edit action triggered for: ${u.username}`);
      },
    },
    {
      label: "Delete",
      className: "btn-action btn-action-danger",
      onClick: (u) => {
        alert(`Delete action triggered for: ${u.username}`);
      },
    },
  ];

  return (
    <BaseListing<User>
      title="Registered Users"
      items={users}
      columns={columns}
      actions={actions}
      emptyTitle="No Registered Users"
      emptyDescription="Use the credentials panel to create a new user profile."
      keyExtractor={(u) => u.id}
    />
  );
}
