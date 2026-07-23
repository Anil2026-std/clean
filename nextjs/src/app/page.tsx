import Link from "next/link";

export const metadata = {
  title: "Home",
  description: "Welcome to our application.",
};

export default function HomePage() {
  return (
    <div className="page-container">
      <header className="page-header">
        <div className="header-text">
          <h2>Home</h2>
          <p>Welcome to our application.</p>
        </div>
        <div style={{ display: "flex", gap: "1.5rem" }}>
          <Link href="/" style={{ fontWeight: 600 }}>
            Home
          </Link>
          <Link href="/list" style={{ fontWeight: 600 }}>
            User Directory
          </Link>
          <Link href="/about" style={{ fontWeight: 600 }}>
            About
          </Link>
          <Link href="/privacy" style={{ fontWeight: 600 }}>
            Privacy
          </Link>
        </div>
      </header>
      <div className="glass-card">
        <p>This page is Home</p>
      </div>
    </div>
  );
}


