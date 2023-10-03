import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import { rest, server } from '../../../testServer';
import AgentOrdersCard from '../OrdersCard/AgentOrdersCard';
import { productdata } from '../../../pages/Orders/Order-Detail.jsx/ProductCard/testdata/Data';
import { orderdetails } from '../../../pages/Orders/Order-Detail.jsx/testData/data';

server.use(
  rest.get('https://localhost:8080/api/agent-order/5', (res, req, ctx) => {
    return res(ctx.status(200), ctx.json(productdata));
  })
);

jest.mock('../../../assets/Image_not_available.png');

describe('orderscard component', () => {
  test('renders order details correctly', () => {
    render(
      <Router>
        <AgentOrdersCard item={orderdetails.data.items[0]} />
      </Router>
    );
    const AgentOrdersCards = screen.getByTestId('AgentOrdersCard');
    expect(AgentOrdersCards).toBeDefined();
  });
});
