import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render } from '@testing-library/react';
import LoginProtectionAgent from '../LoginProtectionAgent';


jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn(),
  useRoutes: jest.fn(),
  Navigate: jest.fn(),
  useLocation: () => ({
    pathname: 'localhost:3000/example/path',
  }),
}));
describe('Loginprotection', () => {
  test('render loginprotectionagent component without accessToken', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
   
    localStorage.setItem('role', '0');
    render(<LoginProtectionAgent />);
  });
  test('render loginprotectionagent component with accessToken and role 1', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '1');
    localStorage.setItem('isInactive','true')
    render(<LoginProtectionAgent />);
  });
  test('render loginprotectionagent component with accessToken and role 3', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');

    localStorage.setItem('role', '3');
    render(<LoginProtectionAgent />);
  });
});
