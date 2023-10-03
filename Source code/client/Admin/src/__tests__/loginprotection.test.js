import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render } from '@testing-library/react';
import LoginProtection from '../LoginProtection';


jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn(),
  Navigate: jest.fn(),
  useRoutes: jest.fn(),
  useLocation: () => ({
    pathname: 'localhost:3000/example/path',
  }),
}));
describe('Loginprotection', () => {
  test('render loginprotection component without accessToken', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '0');
    render(<LoginProtection />);
  });
  test('render loginprotection component with accessToken and role 1', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '1');
    render(<LoginProtection />);
  });
  test('render loginprotection component with accessToken and role 3', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '3');
    render(<LoginProtection />);
  });
});
