import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import UserList from '../Userlist';
import { rest, server } from '../../../testServer';
import {
  alluserdata,
  deleteUserResponse,
  getSingleUserData,
  getSingleUserDataUnblock,
  getUserListData,
} from '../testData/data';
import Swal from 'sweetalert2';
import userEvent from '@testing-library/user-event';

jest.mock('sweetalert2', () => ({
  fire: jest.fn().mockResolvedValue({ isConfirmed: true }),
}));

const UserlistWrapper = ({ handleSort, handleSearch, handleStatus,handlePage}) => {
  return (
    <Router>
      <UserList setSort={handleSort} handleSearch={handleSearch} handleStatus={handleStatus} handlePage={handlePage}  />
    </Router>
  );
};
server.use(
  rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getUserListData));
  })
);
describe('userlist component', () => {
test('should render User list component', async() => {
  render(
    <Router>
      <UserList />
    </Router>
  );
  const userListElement = screen.getByTestId('userlistpage');
  expect(userListElement).toBeDefined();

  const profileicon=await screen.findByText('testuser2');
  expect(profileicon).toBeInTheDocument();
  fireEvent.click(profileicon)
});


test('should render User list component with error response', () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const userListElement = screen.getByTestId('userlistpage');
  expect(userListElement).toBeDefined();
});

test('remove a user  from user list when delete button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleUserData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/User/status/19', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteUserResponse));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const element = await screen.findByText(/testuser2/i);
  expect(element).toBeInTheDocument();
  const deletebtn = screen.getByTestId('deletebtn');
  expect(deletebtn).toBeInTheDocument();
  fireEvent.click(deletebtn);
  await expect(Swal.fire).toBeCalled();
});

test('remove a user  from user list when delete button is clicked get error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleUserData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/User/status/19', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const element = await screen.findByText(/testuser2/i);
  expect(element).toBeInTheDocument();
  const deletebtn = screen.getByTestId('deletebtn');
  expect(deletebtn).toBeInTheDocument();
  fireEvent.click(deletebtn);
});

//block//
test('block a user  from user list when block button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleUserData));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/19', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteUserResponse));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const element = await screen.findByText(/testuser2/i);
  expect(element).toBeInTheDocument();

  const blockbtn = screen.getByTestId('blockbtn');
  expect(blockbtn).toBeInTheDocument();

  fireEvent.click(blockbtn);
  await expect(Swal.fire).toBeCalled();
});

test('block a user  from user list when block button is clicked get error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleUserData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/User/status/19', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const element = await screen.findByText(/testuser2/i);
  expect(element).toBeInTheDocument();
  const blockbtn = screen.getByTestId('blockbtn');
  expect(blockbtn).toBeInTheDocument();
  fireEvent.click(blockbtn);
});

test('Unblock a user  from user list when Unblock button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?Role=0&Role=1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSingleUserDataUnblock));
    })
  );

  server.use(
    rest.put('https://localhost:8080/api/User/status/19', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteUserResponse));
    })
  );
  render(
    <Router>
      <UserList />
    </Router>
  );
  const element = await screen.findByText(/testuser2/i);
  expect(element).toBeInTheDocument();

  const unblockbtn = screen.getByTestId('unblockbtn');
  expect(unblockbtn).toBeInTheDocument();

  fireEvent.click(unblockbtn);
  await expect(Swal.fire).toBeCalled();
});

test('search ,sort filter working', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(alluserdata));
    })
  );
  localStorage.clear();

  const handleSort = jest.fn();
  const handleStatus = jest.fn();
  const handleSearch = jest.fn();


  const user = userEvent.setup();

  render(<UserlistWrapper setSort={handleSort} handleStatus={handleStatus} handleSearch={handleSearch} />);
  const sellerListElement = screen.getByTestId('userlistpage');
  expect(sellerListElement).toBeDefined();

  const data = await screen.findByText(/Test12345/i);
  expect(data).toBeInTheDocument();

  await user.type(screen.getByTestId(/search-input/i), 'Test12345');
  expect(screen.getByTestId(/search-input/i)).toHaveValue('Test12345');

  const statusfilter = await waitFor(() => screen.findByTestId('status-filter-dropdown'), {
    timeout: 3000,
  });
  expect(statusfilter).toBeInTheDocument();

  fireEvent.change(statusfilter, {
    target: { value: 'Active' },
  });
  fireEvent.change(statusfilter, {
    target: { value: 'Inactive' },
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

  const page = await waitFor(() => screen.findByTestId('paginationbutton'), {
    timeout: 3000,
  });
  expect(page).toBeInTheDocument();


});
})