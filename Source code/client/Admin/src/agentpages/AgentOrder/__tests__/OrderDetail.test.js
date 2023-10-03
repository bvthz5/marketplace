import { render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import { rest, server } from '../../../testServer';
import '@testing-library/jest-dom/extend-expect';
import AgentOrderDetailView from '../OrderDetailView/AgentOrderDetailView';
import { OrderDetailview } from '../testData/data';


server.use(
    rest.get('https://localhost:8080/api/agent-order/', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(OrderDetailview));
    })
  );

  test('render agent orderDetail', () =>{
    render(
        <Router>
            <AgentOrderDetailView/>
        </Router>
    );

    const orderDetailElement = screen.getAllByTestId('orderdetail');
    expect(orderDetailElement).toBeDefined();
  });

  test('order detail with error response',()=>{
    server.use(
      rest.get('https://localhost:8080/api/agent-order/', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(OrderDetailview));
      })
    );
    server.use(
      rest.get('https://localhost:8080/api/agent-order/', (req, res, ctx) => {
        return res(ctx.status(400), );
      })
    );
    render(
      <Router>
         <AgentOrderDetailView/>
      </Router>
  );
  
  });



  
  