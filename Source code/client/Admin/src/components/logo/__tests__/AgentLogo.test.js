import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import AgentLogo from '../AgentLogo';
import '@testing-library/jest-dom'; // Import the jest-dom library

describe('AgentLogo component', () => {
  test('renders logo with link', () => {
    render(
      <Router>
        <AgentLogo />
      </Router>
    );
    const linkNode = screen.getByRole('link');
    expect(linkNode).toHaveAttribute('href', '/agentdashboard/home');
  });
  test('renders logo without link', () => {
    render(<AgentLogo disabledLink />);
    const logoNode = screen.getByTestId('agentlogo');
    expect(logoNode).toBeInTheDocument();
  });
});


