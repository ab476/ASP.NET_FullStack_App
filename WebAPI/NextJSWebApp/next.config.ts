import type { NextConfig } from "next";

const API_URL = process.env["services__auth-api__https__0"];

console.log("🔗 API URL (from Aspire):", API_URL ?? "(not provided)");

if (!API_URL) {
  throw new Error(
    "Missing environment variable: services__auth-api__https__0.\n" +
    "Aspire must inject this value. Fix AppHost configuration."
  );
}

const nextConfig: NextConfig = {
  /* config options here */
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${API_URL}/api/:path*`,
      }
    ];
  },
};

export default nextConfig;
