import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render } from '@testing-library/react';
import RouteProtection from '../RouteProtection';


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
  test('render RouteProtection component with accessToken and role 0', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '0');
    render(<RouteProtection />);
  });

  test('render RouteProtection component with role 1', async () => {
    localStorage.clear();
    localStorage.setItem('accessToken', 'testtoken');
    localStorage.setItem('role', '3');
    render(<RouteProtection />);
  });


});
