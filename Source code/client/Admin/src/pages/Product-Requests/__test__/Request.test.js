import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import Request from '../Requests';
import { rest, server } from '../../../testServer';
import { getRequestData } from '../testData/data';

test('should render request component', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/page?Status=2&pageSize=25&pageNumber=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getRequestData));
    })
  );

  render(
    <Router>
      <Request />
    </Router>
  );
  const requestElement = screen.getByTestId('requestpage');
  expect(requestElement).toBeDefined();

  const productdata = await screen.findByText('OnePlus 11 5G (Eternal Green, 16GB RAM, 256GB Storage)');
  expect(productdata).toBeInTheDocument();

  const detailsbutton=screen.getByTestId('details');
  expect(detailsbutton).toBeInTheDocument();
  fireEvent.click(detailsbutton);
});
test('should render request component with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/page?Status=2&pageSize=25&pageNumber=1', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );

  render(
    <Router>
      <Request />
    </Router>
  );
  const requestElement = screen.getByTestId('requestpage');
  expect(requestElement).toBeDefined();


});
