import React from "react";
import { Navigate, Outlet } from "react-router-dom";

export const useLoginAuth = () => {
  const token = localStorage.getItem("accessToken");
  return !!token;
};

const LoginProtection = () => {
  const auth = useLoginAuth();

  return auth ? <Navigate to="/home" data-testid="homepage" /> : <Outlet />;
};

export default LoginProtection;
