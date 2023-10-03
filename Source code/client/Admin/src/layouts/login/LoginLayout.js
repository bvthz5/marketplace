import { Outlet } from 'react-router-dom';
import LoginProtection from '../../../src/LoginProtection';

export default function LoginLayout() {
  return (
    <>
      <LoginProtection>
        <Outlet />
      </LoginProtection>
    </>
  );
}
