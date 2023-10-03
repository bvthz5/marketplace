import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import Logo from '../Logo';
import '@testing-library/jest-dom'; // Import the jest-dom library

describe('Logo component', () => {
  test('renders logo with link', () => {
    render(
      <Router>
        <Logo />
      </Router>
    );
    const linkNode = screen.getByRole('link');
    expect(linkNode).toHaveAttribute('href', '/dashboard/home');
  });
  test('renders logo without link', () => {
    render(<Logo disabledLink />);
    const logoNode = screen.getByTestId('logo');
    expect(logoNode).toBeInTheDocument();
  });
});


