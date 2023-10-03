import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import NoRequestFound from '../NoRequestFound';

test('should render Norequestfound component', () => {
  render(
      <Router>
        <NoRequestFound />
      </Router>
  );
  const noRequestFoundElement = screen.getByTestId('norequestfoundpage');
  expect(noRequestFoundElement).toBeDefined();
});
