import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import OrderDetail from '../OrderDetail';
import { rest, server } from '../../../../../src/testServer';
import { getProductDetailData, orderdetails } from '../testData/data';
import '@testing-library/jest-dom/extend-expect';

server.use(
  rest.get('https://localhost:8080/api/Order/', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductDetailData));
  })
);
test('should render orderDetail component', () => {
  render(
      <Router>
        <OrderDetail />
      </Router>
  );
  const orderDetailElement = screen.getByTestId('orderdetailpage');
  expect(orderDetailElement).toBeDefined();
});

test('order detail with error response',()=>{
  server.use(
    rest.get('https://localhost:8080/api/Order/', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Order/', (req, res, ctx) => {
      return res(ctx.status(400), );
    })
  );
  render(
    <Router>
      <OrderDetail />
    </Router>
);
});
