export type AuthEvent =
  | { type: "TOKEN_UPDATED"; token: string | null }
  | { type: "LOGOUT" };

export type AuthEventHandler = (event: AuthEvent) => void;

export class AuthEvents {
  private channel: BroadcastChannel | null = null;
  private readonly name = "auth_channel_v1";

  constructor() {
    if (typeof window !== "undefined" && "BroadcastChannel" in window) {
      this.channel = new BroadcastChannel(this.name);
    }
  }

  broadcast(event: AuthEvent) {
    if (this.channel) {
      this.channel.postMessage(event);
    } else {
      localStorage.setItem(this.name, JSON.stringify(event));
      localStorage.removeItem(this.name);
    }
  }

  subscribe(handler: AuthEventHandler) {
    if (this.channel) {
      this.channel.onmessage = (ev) => handler(ev.data);
    } else {
      window.addEventListener("storage", (ev) => {
        if (ev.key === this.name && ev.newValue) handler(JSON.parse(ev.newValue));
      });
    }
  }
}

export const authEvents = new AuthEvents();
