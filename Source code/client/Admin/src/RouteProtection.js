import { Navigate } from 'react-router-dom';
import React from 'react';

const RouteProtection = (props) => {
  const token = localStorage.getItem('accessToken');
  const role = localStorage.getItem('role');
  if (token && role === '0') {
    return <>{props.children}</>;
  }
  return <Navigate to="/login" />;
};

export default RouteProtection;
