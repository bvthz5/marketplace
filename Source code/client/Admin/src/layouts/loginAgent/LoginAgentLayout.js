import { Outlet } from 'react-router-dom';
import LoginProtectionAgent from '../../../src/LoginProtectionAgent';

export default function LoginAgentLayout() {
  return (
    <>
      <LoginProtectionAgent>
        <Outlet />
      </LoginProtectionAgent>
    </>
  );
}
