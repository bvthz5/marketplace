import {  render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import MyProducts from '../MyProducts';
import { rest, server } from './../../../../src/testServer';
import { countdata, getProductData, statusdata } from '../testData/data';

import '@testing-library/jest-dom/extend-expect';


server.use(
  rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductData));
  })
);

test('should render myproducts component', async() => {
  render(
      <Router>
        <MyProducts />
      </Router>
  );
  const myProductsElement = screen.getByTestId('myProductspage');
  expect(myProductsElement).toBeDefined();
});

test('count of seller products', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/user/seller-product-status-count/2', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(countdata));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/user/seller-product-status-count/null', (req, res, ctx) => {
      return res(ctx.status(404));
    })
  );
  render(
    <Router>
      <MyProducts />
    </Router>
  );
  const element = await screen.findByText(/26/i);
  expect(element).toBeInTheDocument();
});


test("status of products with success response",async ()=>{
  server.use(
    rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(statusdata));
    })
  );
  render(
    <Router>
      <MyProducts />
    </Router>
  );
  const element =await screen.findByText(/Pending for approval/i)
  expect(element).toBeInTheDocument();
});