import { Navigate, useLocation } from 'react-router-dom';
import React from 'react';

const RouteProtectionAgent = (props) => {
  const token = localStorage.getItem('accessToken');
  const role = localStorage.getItem('role');
  const isInactive = localStorage.getItem('isInactive');

  const location = useLocation();

  if (location.pathname === '/changepassword') {
    if (isInactive === 'true') return <>{props.children}</>;
    return <Navigate to="/agentdashboard/changepassword" />;
  }

  if (token && role === '1') {
    if (isInactive !== 'true') return <>{props.children}</>;
    if (isInactive === 'true') return <Navigate to="/changepassword" />;
  }
  return <Navigate to="/agent/login" />;
};

export default RouteProtectionAgent;
