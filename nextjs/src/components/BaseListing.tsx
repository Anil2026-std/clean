import React from "react";

export interface Column<T> {
  header: string;
  key?: keyof T;
  render?: (item: T) => React.ReactNode;
  width?: string;
}

export interface Action<T> {
  label: string | React.ReactNode;
  onClick: (item: T) => void;
  className?: string;
}

interface BaseListingProps<T> {
  title: string;
  items: T[];
  columns: Column<T>[];
  actions?: Action<T>[];
  emptyTitle?: string;
  emptyDescription?: string;
  keyExtractor: (item: T) => string;
}

export default function BaseListing<T>({
  title,
  items,
  columns,
  actions = [],
  emptyTitle = "No items found",
  emptyDescription = "There are no entries to display.",
  keyExtractor,
}: BaseListingProps<T>) {
  return (
    <div className="glass-card">
      <h3 className="card-title">{title}</h3>

      {items.length === 0 ? (
        <div className="state-container">
          <h5>{emptyTitle}</h5>
          <p>{emptyDescription}</p>
        </div>
      ) : (
        <div className="users-table-container">
          <table className="users-table">
            <thead>
              <tr>
                {columns.map((col, idx) => (
                  <th key={idx} style={col.width ? { width: col.width } : undefined}>
                    {col.header}
                  </th>
                ))}
                {actions.length > 0 && <th style={{ textAlign: "right" }}>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={keyExtractor(item)} className="user-row">
                  {columns.map((col, idx) => (
                    <td key={idx}>
                      {col.render
                        ? col.render(item)
                        : col.key
                        ? String(item[col.key] ?? "")
                        : null}
                    </td>
                  ))}
                  {actions.length > 0 && (
                    <td style={{ textAlign: "right" }}>
                      <div style={{ display: "inline-flex", gap: "0.5rem", justifyContent: "flex-end" }}>
                        {/* {actions.map((act, actIdx) => (
                          <button
                            key={actIdx}
                            onClick={() => act.onClick(item)}
                            className={act.className || "btn-action"}
                          >
                            {act.label}
                          </button>
                        ))} */}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
