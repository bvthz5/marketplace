import axios from "axios";
import { refreshToken } from "./coreApiService";
import { toast } from "react-toastify";

const URL = process.env.REACT_APP_API_PATH
  ? process.env.REACT_APP_API_PATH
  : "https://localhost:8080";

export const axiosInstance = axios.create({
  baseURL: `${URL}`,
  headers: {
    "Content-type": "application/json",
  },
});

axiosInstance.interceptors.request.use((request) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    request.headers.Authorization = `Bearer ${token}`;
  }
  return request;
});

axiosInstance.interceptors.response.use(
  (response) => {
    return response;
  },
  async function (error) {
    let serverErr = "Network Error";
    if (error.message === serverErr) {
      toast.error("Server Connection failed!", {
        toastId: 1,
        theme: "colored",
      });
    }
    const originalRequest = error.config;
    if (
      error.response.status === 401 &&
      !originalRequest._retry &&
      error.config.url !== "/api/Login"
    ) {
      originalRequest._retry = true;
      const accessToken = await refreshAccessToken();
      axiosInstance.defaults.headers.common["Authorization"] =
        "Bearer " + accessToken;
      return axiosInstance(originalRequest);
    }
    return Promise.reject(error);
  }
);

const refreshAccessToken = async () => {
  refreshToken()
    .then((response) => {
      const accessToken = response?.data?.data?.accessToken?.value;
      const newRefreshToken = response?.data?.data?.refreshToken?.value;
      localStorage.setItem("accessToken", accessToken);
      localStorage.setItem("refreshToken", newRefreshToken);
      return accessToken;
    })
    .catch((err) => {
      console.log(err);
      localStorage.clear();
      console.log(err?.response?.data?.message, "no refreshhhhh");
      window.location.replace("/login");
      toast.error("Session Expired! Login Again", { toastId: 2 });
      return null;
    });
};
