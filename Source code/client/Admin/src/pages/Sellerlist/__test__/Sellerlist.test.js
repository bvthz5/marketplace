import { fireEvent, render, screen,waitFor } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import SellerList from '../Sellerlist';
import { rest, server } from '../../../testServer';
import userEvent from '@testing-library/user-event';
import {
  allsellerdata,
  deleteSellerResponse,
  getSellerlistData,
  getSingleSellerData,
  getSingleSellerDataUnblock,
} from '../testData/data';
import Swal from 'sweetalert2';

jest.mock('../../../assets/Screenshot-2018-12-16-at-21.06.29.png')
jest.mock('sweetalert2', () => ({
  fire: jest.fn().mockResolvedValue({ isConfirmed: true }),
}));


const SellerlistWrapper = ({ handleSort, handleSearch, handleStatus }) => {
  return (
    <Router>
      <SellerList setSort={handleSort} handleSearch={handleSearch} handleStatus={handleStatus} />
    </Router>
  );
};
server.use(
  rest.get('https://localhost:8080/api/User/page', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getSellerlistData));
  })
);
describe('sellerlist component', () => {
test('get all sellers', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
});

jest.mock('sweetalert2', () => ({
  fire: jest.fn().mockResolvedValue({ isConfirmed: true }),
}));

test('should render Seller listcomponent', () => {
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const sellerListElement = screen.getByTestId('sellerlistpage');
  expect(sellerListElement).toBeDefined();
});

test('remove a user  from user list when delete button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleSellerData));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/14', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteSellerResponse));
    })
  );
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const element = await screen.findByText(/Test5/i);
  expect(element).toBeInTheDocument();

  const deletebtn = screen.getByTestId('deletebtn');
  expect(deletebtn).toBeInTheDocument();

  fireEvent.click(deletebtn);
  await expect(Swal.fire).toBeCalled();
});

test('remove a user  from user list when delete button is clicked get error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleSellerData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/User/status/14', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const element = await screen.findByText(/Test5/i);
  expect(element).toBeInTheDocument();

  const deletebtn = screen.getByTestId('deletebtn');
  expect(deletebtn).toBeInTheDocument();

  fireEvent.click(deletebtn);
});

//block//
test('block a user  from user list when block button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleSellerData));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/14', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteSellerResponse));
    })
  );
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const element = await screen.findByText(/Test5/i);
  expect(element).toBeInTheDocument();

  const blockbtn = screen.getByTestId('blockbtn');
  expect(blockbtn).toBeInTheDocument();

  fireEvent.click(blockbtn);
  await expect(Swal.fire).toBeCalled();
});

test('block a user  from user list when block button is clicked get error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleSellerData));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/14', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const element = await screen.findByText(/Test5/i);
  expect(element).toBeInTheDocument();

  const blockbtn = screen.getByTestId('blockbtn');
  expect(blockbtn).toBeInTheDocument();

  fireEvent.click(blockbtn);
});

test('Unblock a user  from user list when   Unblock button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleSellerDataUnblock));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/14', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteSellerResponse));
    })
  );
  render(
    <Router>
      <SellerList />
    </Router>
  );
  const element = await screen.findByText(/Test5/i);
  expect(element).toBeInTheDocument();

  const unblockbtn = screen.getByTestId('unblockbtn');
  expect(unblockbtn).toBeInTheDocument();

  fireEvent.click(unblockbtn);
  await expect(Swal.fire).toBeCalled();
});

test('search ,sort filter working', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(allsellerdata));
    })
  );
  localStorage.clear();

  const handleSort = jest.fn();
  const handleStatus = jest.fn();
  const handleSearch = jest.fn();

  const user = userEvent.setup();

  render(<SellerlistWrapper setSort={handleSort} handleStatus={handleStatus} handleSearch={handleSearch} />);
  const sellerListElement = screen.getByTestId('sellerlistpage');
  expect(sellerListElement).toBeDefined();

  const data = await screen.findByText(/Test5/i);
  expect(data).toBeInTheDocument();

  await user.type(screen.getByTestId(/search-input/i), 'Test5');
  expect(screen.getByTestId(/search-input/i)).toHaveValue('Test5');

  const statusfilter = await waitFor(() => screen.findByTestId('status-filter-dropdown'), {
    timeout: 3000,
  });
  expect(statusfilter).toBeInTheDocument();

  fireEvent.change(statusfilter, {
    target: { value: 'Active' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'Blocked' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'Deleted' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'All' },
  });


  const sort = await waitFor(() => screen.findByTestId('sort-dropdown'), {
    timeout: 3000,
  });
  expect(sort).toBeInTheDocument();

  fireEvent.change(sort, {
    target: { value: 'Created Date' },
  });

  sort.dispatchEvent(new Event('change'));

  fireEvent.change(sort, {
    target: { value: 'First Name' },
  });

  sort.dispatchEvent(new Event('change'));
  fireEvent.change(sort, {
    target: { value: 'Email' },
  });

  sort.dispatchEvent(new Event('change'));
});

});