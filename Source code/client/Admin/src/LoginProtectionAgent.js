import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';

const LoginProtectionAgent = () => {
  const token = localStorage.getItem('accessToken');
  const role = localStorage.getItem('role');
  const status = localStorage.getItem('isInactive');

  if (token && role === '1') {
    if (status !== 'true') return <Navigate to="/agentdashboard/home" />;
    if (status === 'true') return <Navigate to="changepassword" />;
  }
  if (token && role === '0') {
    return <Navigate to="/dashboard/home" />;
  }
  return <Outlet />;
};

export default LoginProtectionAgent;
