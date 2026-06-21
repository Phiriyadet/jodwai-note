import axios from "axios";

export const httpClient = axios.create({
  baseURL: "https://localhost:7178",
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
