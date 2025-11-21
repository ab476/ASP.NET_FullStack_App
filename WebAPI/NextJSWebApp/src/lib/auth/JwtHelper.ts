export class JwtHelper {
  decode(token: string): any | null {
    try {
      const parts = token.split(".");
      if (parts.length !== 3) return null;
      return JSON.parse(Buffer.from(parts[1], "base64").toString());
    } catch {
      return null;
    }
  }

  expiryMs(token: string): number | null {
    const payload = this.decode(token);
    return payload?.exp ? payload.exp * 1000 : null;
  }

  msUntilExpiry(token: string): number | null {
    const exp = this.expiryMs(token);
    return exp ? exp - Date.now() : null;
  }
}

export const jwtHelper = new JwtHelper();
