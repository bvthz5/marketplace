import '@testing-library/jest-dom/extend-expect';
import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import { rest, server } from '../../../testServer';
import AgentOrderList from '../OrderList/AgentOrderList';
import { orderlistdataagent } from '../testData/data';

server.use(
  rest.get('https://localhost:8080/api/agent-order/page', (res, req, ctx) => {
    return res(ctx.status(200), ctx.json(orderlistdataagent));
  })
);


describe('orderlist component', () => {
  test('render agent orderlist', () => {
    render(
      <Router>
        <AgentOrderList />
      </Router>
    );

    const orderListElement = screen.getAllByTestId('orderlist');
    expect(orderListElement).toBeDefined();
  });

 });
