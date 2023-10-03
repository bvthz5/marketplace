import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import AgentDashboard from '../AgentDashboard';


test('should render AgentDashboard component', () => {
  render(
      <Router>
        <AgentDashboard />
      </Router>
  );
  // const AgentDashboardElement = screen.getByTestId('agentdashboardpage');
  // expect( AgentDashboardElement).toBeDefined();
});
