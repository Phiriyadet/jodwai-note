import axios from "axios";

export const httpClient = axios.create({
  baseURL: "http://localhost:5105",
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
