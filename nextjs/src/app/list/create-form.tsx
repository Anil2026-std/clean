"use client";

import { useState, useRef } from "react";
import { createUserAction } from "@/app/actions";
import Input from "@/components/Input";
import Button from "@/components/Button";

export default function CreateForm() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [imagePreviewUrl, setImagePreviewUrl] = useState<string | null>(null);
  const [uploadError, setUploadError] = useState<string | null>(null);
  
  const [isLoading, setIsLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Real-time password rules
  const hasMinLength = password.length >= 8;
  const hasUpper = /[A-Z]/.test(password);
  const hasNumber = /[0-9]/.test(password);
  const hasSpecial = /[^A-Za-z0-9]/.test(password);
  
  const isPasswordValid = hasMinLength && hasUpper && hasNumber && hasSpecial;
  const isFormValid = username.length >= 3 && isPasswordValid;

  const handleFileSelected = (e: React.ChangeEvent<HTMLInputElement>) => {
    setUploadError(null);
    const file = e.target.files?.[0];
    if (file) {
      const maxFileSize = 10 * 1024 * 1024; // 10MB
      if (file.size > maxFileSize) {
        setUploadError("File exceeds the 10MB size limit.");
        return;
      }

      const allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
      const extension = file.name.substring(file.name.lastIndexOf(".")).toLowerCase();
      if (!allowedExtensions.includes(extension)) {
        setUploadError("Allowed file types: JPG, JPEG, PNG, GIF, WEBP.");
        return;
      }

      setSelectedFile(file);

      // Generate preview
      const reader = new FileReader();
      reader.onload = (event) => {
        setImagePreviewUrl(event.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const clearSelectedFile = () => {
    setSelectedFile(null);
    setImagePreviewUrl(null);
    setUploadError(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid || isLoading) return;

    setIsLoading(true);
    setSuccessMessage(null);
    setErrorMessage(null);

    const formData = new FormData();
    formData.append("username", username);
    formData.append("password", password);
    if (selectedFile) {
      formData.append("file", selectedFile);
    }

    try {
      const response = await createUserAction(null, formData);
      if (response.success) {
        setSuccessMessage(response.message || "User created successfully!");
        setUsername("");
        setPassword("");
        clearSelectedFile();
      } else {
        // Display validation errors if available, otherwise general message
        if (response.errors) {
          const errList = Object.entries(response.errors)
            .map(([prop, messages]) => `${prop}: ${messages.join(", ")}`)
            .join("; ");
          setErrorMessage(errList || response.message || "Failed to create user.");
        } else {
          setErrorMessage(response.message || "Failed to create user.");
        }
      }
    } catch (err: any) {
      setErrorMessage("An unexpected error occurred. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="glass-card">
      <h3 className="card-title">Create Credentials</h3>

      {errorMessage && (
        <div className="alert-container alert-danger">
          <span>{errorMessage}</span>
        </div>
      )}

      {successMessage && (
        <div className="alert-container alert-success">
          <span>{successMessage}</span>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        {/* Profile Image */}
        <div className="form-group-custom">
          <label>Profile Image (Optional)</label>
          <div className="avatar-upload-container">
            <div className="avatar-preview-wrapper">
              {imagePreviewUrl ? (
                <>
                  <img src={imagePreviewUrl} className="avatar-preview" alt="Preview" />
                  <button type="button" className="btn-remove-avatar" onClick={clearSelectedFile}>×</button>
                </>
              ) : (
                <div className="avatar-placeholder">
                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" viewBox="0 0 16 16" style={{ color: "#94a3b8" }}>
                    <path d="M11 6a3 3 0 1 1-6 0 3 3 0 0 1 6 0z"/>
                    <path fillRule="evenodd" d="M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8zm8-7a7 7 0 0 0-5.468 11.37C3.242 11.226 4.805 10 8 10s4.757 1.225 5.468 2.37A7 7 0 0 0 8 1z"/>
                  </svg>
                </div>
              )}
            </div>
            <div className="upload-btn-wrapper">
              <button type="button" className="btn-select-avatar">Choose Image</button>
              <input
                type="file"
                ref={fileInputRef}
                onChange={handleFileSelected}
                accept="image/png, image/jpeg, image/gif, image/webp"
                disabled={isLoading}
              />
            </div>
          </div>
          {uploadError && <span className="error-text">{uploadError}</span>}
        </div>

        {/* Username */}
        <Input
          type="text"
          label="Username"
          placeholder="Enter username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          disabled={isLoading}
          id="username"
          required
        />

        {/* Password */}
        <Input
          type="password"
          label="Password"
          placeholder="Enter secure password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          disabled={isLoading}
          id="password"
          required
        />

        {/* Password Rules checklist */}
        <ul className="rules-list">
          <li className={`rule-item ${hasMinLength ? "text-success" : "text-danger"}`}>
            • At least 8 characters
          </li>
          <li className={`rule-item ${hasUpper ? "text-success" : "text-danger"}`}>
            • One uppercase letter
          </li>
          <li className={`rule-item ${hasNumber ? "text-success" : "text-danger"}`}>
            • One number
          </li>
          <li className={`rule-item ${hasSpecial ? "text-success" : "text-danger"}`}>
            • One special character
          </li>
        </ul>

        <Button type="submit" disabled={!isFormValid} isLoading={isLoading} loadingText="Saving User...">
          Add User
        </Button>
      </form>
    </div>
  );
}
