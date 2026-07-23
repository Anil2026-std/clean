import type { NextConfig } from "next";

// Bypass SSL errors for local development (self-signed certificates)
if (process.env.NODE_ENV === "development") {
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";
}

const nextConfig: NextConfig = {
  images: {
    deviceSizes: [640, 750, 828, 1080, 1200],
    imageSizes: [16, 32, 48, 64, 96, 128, 256],
    remotePatterns: [
      {
        protocol: "https",
        hostname: "localhost",
        port: "7180",
        pathname: "/**",
      },
    ],
  },
};

export default nextConfig;

