import React from 'react';
import { render, screen } from '@testing-library/react';
import App from '../App';

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn(),
  useRoutes: jest.fn(),
  useLocation: () => ({
    pathname: 'localhost:3000/example/path',
  }),
}));

describe('App', () => {
  test('renders the app', () => {
    render(<App />);
    const app = screen.getByTestId('app');
    expect(app).toBeDefined();
  });
});
