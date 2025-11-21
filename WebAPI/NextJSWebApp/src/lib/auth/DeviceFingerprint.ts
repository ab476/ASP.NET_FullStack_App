import FingerprintJS, { Agent } from '@fingerprintjs/fingerprintjs';

export class DeviceFingerprint {
  private readonly deviceIdKey = "device_id_v1";
  private readonly fingerprintKey = "fingerprint_v1";

  private deviceId: string | null = null;
  private fingerprint: string | null = null;

  private fpAgent: Agent | null = null;

  constructor() {
    if (typeof window !== "undefined") {
      // load or create deviceId
      this.deviceId = this.loadOrCreate(this.deviceIdKey, this.generateUuid());

      // load fingerprint from storage if already exists
      const storedFp = localStorage.getItem(this.fingerprintKey);
      if (storedFp) this.fingerprint = storedFp;

      // initialize fingerprintjs asynchronously
      this.initFingerprintJS();
    }
  }

  /** Initialize FingerprintJS agent */
  private async initFingerprintJS() {
    try {
      this.fpAgent = await FingerprintJS.load();
    } catch (e) {
      console.error("Failed to load FingerprintJS:", e);
    }
  }

  /** Create persistent deviceId */
  private generateUuid(): string {
    return crypto.randomUUID();
  }

  /** Reusable localStorage load-or-create logic */
  private loadOrCreate(key: string, value: string): string {
    const existing = localStorage.getItem(key);
    if (existing) return existing;

    localStorage.setItem(key, value);
    return value;
  }

  /** Returns persistent device ID */
  getDeviceId(): string | null {
    return this.deviceId;
  }

  /** Returns stable FingerprintJS ID */
  async getFingerprint(): Promise<string | null> {
    if (this.fingerprint) return this.fingerprint;

    // Ensure FPJS agent is ready
    if (!this.fpAgent) {
      this.fpAgent = await FingerprintJS.load();
    }

    try {
      const result = await this.fpAgent.get();
      const visitorId = result.visitorId;

      this.fingerprint = visitorId;
      localStorage.setItem(this.fingerprintKey, visitorId);

      return visitorId;
    } catch (err) {
      console.error("Failed to generate fingerprint:", err);
      return null;
    }
  }
}

export const deviceFingerprint = new DeviceFingerprint();
