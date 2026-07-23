"use client";

import { useState } from "react";
import { loginAction } from "@/app/actions";
import Input from "@/components/Input";
import Button from "@/components/Button";

export default function LoginForm() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !password) return;

    setIsLoading(true);
    setErrorMessage(null);

    const formData = new FormData();
    formData.append("username", username);
    formData.append("password", password);

    try {
      const response = await loginAction(null, formData);
      if (response && !response.success) {
        setErrorMessage(response.message || "Invalid username or password.");
      }
    } catch (err: any) {
      setErrorMessage("An unexpected error occurred. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (

    <div className="login-card">
      <div className="login-header">
        <h2>Welcome Back</h2>
        <p>Please sign in to your account</p>
      </div>


      {errorMessage && (
        <div className="alert-feedback alert-feedback-danger">
          <span>{errorMessage}</span>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <Input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          disabled={isLoading}
          required
          id="username"
          wrapperClassName="input-group-custom"
        />

        <Input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={isLoading}
          required
          id="password"
          wrapperClassName="input-group-custom"
        />

        <Button
          type="submit"
          variant="login"
          isLoading={isLoading}
          loadingText="Signing In..."
        >
          Sign In
        </Button>
      </form>
    </div>
  );
}
