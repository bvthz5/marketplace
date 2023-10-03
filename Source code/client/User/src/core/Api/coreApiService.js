import { axiosInstance } from "./interceptor";




export const refreshToken = () => {
  const refreshToken = localStorage.getItem("refreshToken");
  return axiosInstance.put(`/api/Login`, JSON.stringify(refreshToken));
};

// ----------------------------------------------------------------------------------------------------------------------------------
// registration apis

export const registerUser = (data) => {
  return axiosInstance.post(`/api/User`, JSON.stringify(data));
};

// ----------------------------------------------------------------------------------------------------------------------------------

// ----------------------------------------------------------------------------------------------------------------------------------
// email verification apis (token)

export const tokenVerification = (token) => {
  return axiosInstance.put(`/api/User/verify`, JSON.stringify(token));
};

export const resendVerificationMail = (email) => {
  return axiosInstance.put(
    `/api/User/resend-verification-mail`,
    JSON.stringify(email)
  );
};

export const resendVerificationMailByToken = (token) => {
  return axiosInstance.put(
    `/api/user/resend-verification-mail-token`,
    JSON.stringify(token)
  );
};
// ----------------------------------------------------------------------------------------------------------------------------------

// ----------------------------------------------------------------------------------------------------------------------------------
// login apis

export const login = (data) => {
  return axiosInstance.post(`/api/Login`, JSON.stringify(data));
};

export const googleLogin = (userObject) => {
  return axiosInstance.post(`/api/Google/login`, JSON.stringify(userObject));
};
// ----------------------------------------------------------------------------------------------------------------------------------

// ----------------------------------------------------------------------------------------------------------------------------------
// Reset password apis
export const forgotPassword = (email) => {
  return axiosInstance.put(`/api/User/forgot-password`, JSON.stringify(email));
};

export const resetPassword = (token, password) => {
  return axiosInstance.put(
    `/api/User/reset-password`,
    JSON.stringify({ token, password })
  );
};
