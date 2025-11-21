import { tokenStore } from "./TokenStore";
import { httpClient } from "./HttpClient";

export class AuthManager {
  constructor() {
    this.setupProactiveRefresh();
    this.setupFocusRefresh();
  }

  async login(email: string, password: string, deviceId: string, fingerprint: string) {
    const res = await httpClient.instance.post("/auth/login", {
      email,
      password,
      deviceId,
      fingerprint
    });

    tokenStore.setToken(res.data.accessToken);
    return res.data;
  }

  async logout() {
    try {
      await httpClient.instance.post("/auth/logout");
    } catch {}
    tokenStore.clearToken();
  }

  // Refresh every 30 seconds if nearly expiring
  private setupProactiveRefresh() {
    if (typeof window === "undefined") return;

    const loop = () => {
      if (tokenStore.isExpiring(60000)) {
        httpClient.instance.post("/auth/refresh").then((r) => {
          tokenStore.setToken(r.data.accessToken);
        });
      }
      setTimeout(loop, 30000);
    };

    loop();
  }

  private setupFocusRefresh() {
    if (typeof window === "undefined") return;

    window.addEventListener("focus", () => {
      if (tokenStore.isExpiring(60000)) {
        httpClient.instance.post("/auth/refresh").then((r) => {
          tokenStore.setToken(r.data.accessToken);
        });
      }
    });
  }
}

export const authManager = new AuthManager();
