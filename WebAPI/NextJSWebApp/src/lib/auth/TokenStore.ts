// src/lib/auth/TokenStore.ts
import { authEvents, AuthEvent } from "./AuthEvents";
import { jwtHelper } from "./JwtHelper";

export class TokenStore {
  private token: string | null = null;
  private readonly storageKey = "access_token_v1";
  private persistSession = true;

  /** Initialize token sync across tabs */
  init({ persist = true }: { persist?: boolean } = {}) {
    this.persistSession = persist;

    if (typeof window !== "undefined" && persist) {
      this.token = sessionStorage.getItem(this.storageKey);
    }

    // Listen to token updates from other tabs
    authEvents.subscribe((event: AuthEvent) => {
      switch (event.type) {
        case "TOKEN_UPDATED":
          this.setToken(event.token, { broadcast: false });
          break;

        case "LOGOUT":
          this.clearToken({ broadcast: false });
          break;
      }
    });
  }

  /** Returns in-memory access token */
  getToken(): string | null {
    return this.token;
  }

  /** Set token and sync across tabs */
  setToken(token: string | null, opts: { broadcast?: boolean } = {}) {
    this.token = token;

    if (typeof window !== "undefined" && this.persistSession) {
      if (token) sessionStorage.setItem(this.storageKey, token);
      else sessionStorage.removeItem(this.storageKey);
    }

    if (opts.broadcast !== false) {
      authEvents.broadcast({ type: "TOKEN_UPDATED", token });
    }
  }

  /** Clear token everywhere */
  clearToken(opts: { broadcast?: boolean } = {}) {
    this.token = null;

    if (typeof window !== "undefined" && this.persistSession) {
      sessionStorage.removeItem(this.storageKey);
    }

    if (opts.broadcast !== false) {
      authEvents.broadcast({ type: "LOGOUT" });
    }
  }

  /** Whether token expires soon */
  isExpiring(thresholdMs = 60_000): boolean {
    if (!this.token) return false;

    const timeLeft = jwtHelper.msUntilExpiry(this.token);
    return timeLeft !== null && timeLeft < thresholdMs;
  }
}

export const tokenStore = new TokenStore();
