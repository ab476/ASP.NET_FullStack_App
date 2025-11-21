import axios, { AxiosInstance } from "axios";
import { tokenStore } from "./TokenStore";

export class HttpClient {
  instance: AxiosInstance;
  private isRefreshing = false;
  private queue: Array<(t: string) => void> = [];

  constructor(baseURL: string) {
    this.instance = axios.create({
      baseURL,
      withCredentials: true
    });

    this.attachInterceptors();
  }

  private attachInterceptors() {
    // Add access token
    this.instance.interceptors.request.use((config) => {
      const token = tokenStore.getToken();
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Handle refresh on 401
    this.instance.interceptors.response.use(
      (r) => r,
      async (error) => {
        const original = error.config;

        if (error.response?.status !== 401 || original._retry) {
          return Promise.reject(error);
        }

        original._retry = true;

        if (!this.isRefreshing) {
          this.isRefreshing = true;

          try {
            const res = await axios.post(
              `${this.instance.defaults.baseURL}/auth/refresh`,
              {},
              { withCredentials: true }
            );

            const newToken = res.data.accessToken;
            tokenStore.setToken(newToken);

            this.isRefreshing = false;
            this.queue.forEach((cb) => cb(newToken));
            this.queue = [];
          } catch (err) {
            this.isRefreshing = false;
            tokenStore.clearToken();
            return Promise.reject(err);
          }
        }

        return new Promise((resolve) => {
          this.queue.push((t) => {
            original.headers.Authorization = `Bearer ${t}`;
            resolve(this.instance(original));
          });
        });
      }
    );
  }
}

export const httpClient = new HttpClient(process.env.NEXT_PUBLIC_API_URL!);
