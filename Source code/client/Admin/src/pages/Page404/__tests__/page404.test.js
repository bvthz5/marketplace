import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render, screen } from '@testing-library/react';
import Page404 from '../Page404';
import { BrowserRouter as Router } from 'react-router-dom';

jest.mock('react-helmet-async');
describe('Page404', () => {
  it('should render the page not found message', async () => {
    render(
      <Router>
        <Page404 />
      </Router>
    );
    const title = await screen.findByText('Sorry, page not found!');
    const message = await screen.findByText(
      'Sorry, we couldn’t find the page you’re looking for. Perhaps you’ve mistyped the URL? Be sure to check your spelling.'
    );
    expect(title).toBeInTheDocument();
    expect(message).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Go to Home' })).toBeInTheDocument()
  });

  it('should render the home button', async () => {
    render(
        <Router>
          <Page404 />
        </Router>
      );
    const button = await screen.findByText('Go to Home');
    expect(button).toBeInTheDocument();
  });
});
