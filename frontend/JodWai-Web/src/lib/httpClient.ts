import axios from "axios";

export const httpClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  // ?? "https://localhost:7178"
  headers: {
    "Content-Type": "application/json",
  },
});

httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error(error);

    return Promise.reject(error);
  },
);
