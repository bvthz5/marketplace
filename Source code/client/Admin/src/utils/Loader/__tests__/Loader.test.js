import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import Loader from '../Loader';

test('should render loader component', () => {
  render(
      <Router>
        <Loader />
      </Router>
  );
  const LoaderElement = screen.getByTestId('loaderpage');
  expect(LoaderElement).toBeDefined();
});
