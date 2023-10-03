import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render } from '@testing-library/react';
import RouteProtectionAgent from '../RouterProtectionAgent';

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn(),
  useRoutes: jest.fn(),
  Navigate: jest.fn(),
  useLocation: () => ({
    pathname: 'localhost:3000/example/path',
  }),
}));



describe('RouteProtection', () => {
  test('render RouteProtectionAgent component with accessToken and role 1', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('isInactive', 'true');
    localStorage.setItem('role', '1');
    render(<RouteProtectionAgent />);
  });
  test('render RouteProtectionAgent component with accessToken and role 1 isinactive false', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('isInactive', 'false');
    localStorage.setItem('role', '1');
    render(<RouteProtectionAgent />);
  });
  test('render RouteProtectionAgent component with role 3', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '3');
    render(<RouteProtectionAgent />);
  });


});
