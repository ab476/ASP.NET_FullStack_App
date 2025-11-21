import { useEffect, useState } from "react";
import { authManager } from "@/lib/auth/AuthManager";
import { tokenStore } from "@/lib/auth/TokenStore";

export function useAuth() {
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    tokenStore.init({ persist: true });
    setToken(tokenStore.getToken());

    const sync = () => setToken(tokenStore.getToken());
    window.addEventListener("storage", sync);

    return () => window.removeEventListener("storage", sync);
  }, []);

  return {
    token,
    login: authManager.login.bind(authManager),
    logout: authManager.logout.bind(authManager),
    isAuthenticated: !!token
  };
}
