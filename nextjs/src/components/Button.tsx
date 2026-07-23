import React from "react";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  isLoading?: boolean;
  loadingText?: string;
  variant?: "primary" | "login";
}

export default function Button({
  children,
  isLoading,
  loadingText,
  variant = "primary",
  className = "",
  disabled,
  ...props
}: ButtonProps) {
  const baseClass = variant === "login" ? "btn-login" : "btn-submit";
  return (
    <button
      className={`${baseClass} ${className}`}
      disabled={disabled || isLoading}
      {...props}
    >
      <span>{isLoading && loadingText ? loadingText : children}</span>
    </button>
  );
}
