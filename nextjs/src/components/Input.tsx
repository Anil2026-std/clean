import React from "react";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  wrapperClassName?: string;
}

export default function Input({
  label,
  error,
  wrapperClassName = "form-group-custom",
  className = "",
  ...props
}: InputProps) {
  return (
    <div className={wrapperClassName}>
      {label && <label htmlFor={props.id}>{label}</label>}
      <div className="input-wrapper">
        <input
          className={`form-control-custom ${error ? "invalid" : ""} ${className}`}
          {...props}
        />
      </div>
      {error && <span className="error-text">{error}</span>}
    </div>
  );
}
