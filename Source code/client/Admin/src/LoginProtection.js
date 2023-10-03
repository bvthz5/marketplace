import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';

const LoginProtection = () => {
  const token = localStorage.getItem('accessToken');
  const role = localStorage.getItem('role');
  if (token && role === '0') {
    return <Navigate to="/dashboard/home" />;
  }
  if (token && role === '1') {
    return <Navigate to="/agentdashboard/home" />;
  }
  return <Outlet />;
};

export default LoginProtection;
