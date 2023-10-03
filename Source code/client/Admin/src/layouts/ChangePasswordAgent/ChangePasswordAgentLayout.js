import React from 'react';
import { Outlet } from 'react-router-dom';
import RouteProtectionAgent from '../../../src/RouterProtectionAgent';

export default function ChangePasswordAgentLayout() {
  return (
    <>
      <RouteProtectionAgent >
        <Outlet/>
      </RouteProtectionAgent>
    </>
  );
}
