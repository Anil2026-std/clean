import LoginForm from "./login-form";

export const metadata = {
  title: "Login",
  description: "Sign in to access your system directory.",
};

export default function LoginPage() {
  return (
    <div className="login-page-container">
      <div className="glow-orb-1"></div>
      <div className="glow-orb-2"></div>
      <LoginForm />
    </div>
  );
}
