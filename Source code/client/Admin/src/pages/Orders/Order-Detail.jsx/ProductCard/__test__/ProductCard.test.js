import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ProductCard from '../ProductCard';
import { BrowserRouter as Router } from 'react-router-dom';
import { rest, server } from '../../../../../testServer';
import { productdata } from '../testdata/Data';
import { orderdetails } from '../../testData/data';

server.use(
  rest.get('https://localhost:8080/api/Order/22', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(productdata));
  })
);

jest.mock('./../../../../assets/Image_not_available.png');
describe('ProductCard component', () => {
  test('renders product details correctly', () => {
    render(
      <Router>
        <ProductCard item={orderdetails.data.items[0]}/>
      </Router>
    );
    const productcard = screen.getByTestId('productcard');
    expect(productcard).toBeDefined();

  });
});
