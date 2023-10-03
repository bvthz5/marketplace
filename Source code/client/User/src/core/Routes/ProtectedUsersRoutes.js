import React from "react";
import { Navigate, Outlet } from "react-router-dom";

const useAuth = () => {
  const token = localStorage.getItem("accessToken");
  return !!token;
};

const ProtectedUsersRoutes = () => {
  const auth = useAuth();

  return auth ? <Outlet /> : <Navigate to="/" />;
};

export default ProtectedUsersRoutes;
