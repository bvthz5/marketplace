import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import OrderList from '../Orderlist';
import { rest, server } from '../../../testServer';
import { getOrderListData, orderlistdata } from '../testData/data';
import userEvent from '@testing-library/user-event';

window.scrollTo = jest.fn();
server.use(
  rest.get('https://localhost:8080/api/Order/page', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getOrderListData));
  })
);

const OrderlistWrapper = ({ handleSort, handleSearch, handleStatus }) => {
  return (
    <Router>
      <OrderList setSort={handleSort} handleSearch={handleSearch} handleStatus={handleStatus} />
    </Router>
  );
};
test('should render OrderList component', () => {
  render(
    <Router>
      <OrderList />
    </Router>
  );
  const orderListElement = screen.getByTestId('orderlistpage');
  expect(orderListElement).toBeDefined();
});
test('orderlist with error response', () => {
  server.use(
    rest.get('https://localhost:8080/api/Order/page', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
});

server.use(
  rest.get('https://localhost:8080/api/Order/page', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getOrderListData));
  })
);
test('search ,sort filter working', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Order/page', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(orderlistdata));
    })
  );
  localStorage.clear();

  const handleSort = jest.fn();
  const handleStatus = jest.fn();
  const handleSearch = jest.fn();

  const user = userEvent.setup();

  render(<OrderlistWrapper setSort={handleSort} handleStatus={handleStatus} handleSearch={handleSearch} />);
  const sellerListElement = screen.getByTestId('orderlistpage');
  expect(sellerListElement).toBeDefined();

  const data = await screen.findByText(/pay_LY0oWK91uI35NK/i);
  expect(data).toBeInTheDocument();

  await user.type(screen.getByTestId(/search-input/i), 'pay_LY0oWK91uI35NK');
  expect(screen.getByTestId(/search-input/i)).toHaveValue('pay_LY0oWK91uI35NK');

  const statusfilter = await waitFor(() => screen.findByTestId('status-filter-dropdown'), {
    timeout: 3000,
  });
  expect(statusfilter).toBeInTheDocument();

  fireEvent.change(statusfilter, {
    target: { value: 'Paid' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'Unpaid' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'Refunded' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'All' },
  });

  const sort = await waitFor(() => screen.findByTestId('sort-dropdown'), {
    timeout: 3000,
  });
  expect(sort).toBeInTheDocument();

  fireEvent.change(sort, {
    target: { value: ' Order Id' },
  });

  sort.dispatchEvent(new Event('change'));

  fireEvent.change(sort, {
    target: { value: ' Price' },
  });

  sort.dispatchEvent(new Event('change'));
  fireEvent.change(sort, {
    target: { value: 'Order Date' },
  });

  sort.dispatchEvent(new Event('change'));
});
