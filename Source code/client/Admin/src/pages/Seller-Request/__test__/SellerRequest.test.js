import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import SellerRequest from '../SellerRequest';
import { getSellerRequestData } from '../testData/data';
import { rest, server } from '../../../testServer';
import userEvent from '@testing-library/user-event';

server.use(
  rest.get('https://localhost:8080/api/User/page`', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getSellerRequestData));
  })
);

test('should render Seller Request component with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json(getSellerRequestData));
    })
  );
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();

});
test('should render Seller Request component', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/User/seller-request/21`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();
  const sellername = await screen.findByText(/Testseller/i);
  expect(sellername).toBeInTheDocument();

  const rejectbutton=screen.getByTestId('rejectbutton');
  expect(rejectbutton).toBeInTheDocument();
  fireEvent.click(rejectbutton)

  const rejectmodal=await screen.findByText(/Reject Request?/i);
  expect (rejectmodal).toBeInTheDocument();

  const user = userEvent.setup();

  await user.type(screen.getByTestId('reject-reason'), 'aaaahhhhh');
  expect(screen.getByTestId('reject-reason')).toHaveValue('aaaahhhhh');

  const submitreasonbutton=screen.getByTestId('submitreasonbutton')
  expect(submitreasonbutton).toBeInTheDocument();

});

test('should render Seller Request component with error response onsubmut', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/User/seller-request/21`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();
  const sellername = await screen.findByText(/Testseller/i);
  expect(sellername).toBeInTheDocument();

  const rejectbutton=screen.getByTestId('rejectbutton');
  expect(rejectbutton).toBeInTheDocument();
  fireEvent.click(rejectbutton)

  const rejectmodal=await screen.findByText(/Reject Request?/i);
  expect (rejectmodal).toBeInTheDocument();

  const user = userEvent.setup();

  await user.type(screen.getByTestId('reject-reason'), 'aaaahhhhh');
  expect(screen.getByTestId('reject-reason')).toHaveValue('aaaahhhhh');

  const submitreasonbutton=screen.getByTestId('submitreasonbutton')
  expect(submitreasonbutton).toBeInTheDocument();

});
test('should render Seller Request componenent request reject cancel button', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();
  const sellername = await screen.findByText(/Testseller/i);
  expect(sellername).toBeInTheDocument();

  const rejectbutton=screen.getByTestId('rejectbutton');
  expect(rejectbutton).toBeInTheDocument();
  fireEvent.click(rejectbutton)

  const rejectmodal=await screen.findByText(/Reject Request?/i);
  expect (rejectmodal).toBeInTheDocument();

  const user = userEvent.setup();

  await user.type(screen.getByTestId('reject-reason'), 'aaaahhhhh');
  expect(screen.getByTestId('reject-reason')).toHaveValue('aaaahhhhh');

  const cancelreasonbutton=screen.getByTestId('cancelreasonbutton')
  expect(cancelreasonbutton).toBeInTheDocument();

});
test('seller request approve button', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/User/seller-request/${userId}``', (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();
  const sellername = await screen.findByText(/Testseller/i);
  expect(sellername).toBeInTheDocument();

  const approvebutton=screen.getByTestId('approvebutton');
  expect(approvebutton).toBeInTheDocument();
  fireEvent.click(approvebutton)
});
test('seller request approve button with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/page?pageNumber=1&Role=1&pageSize=25&Status=1`', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getSellerRequestData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/User/seller-request/${userId}``', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <SellerRequest />
    </Router>
  );
  const sellerRequestElement = screen.getByTestId('sellerrequestpage');
  expect(sellerRequestElement).toBeDefined();
  const sellername = await screen.findByText(/Testseller/i);
  expect(sellername).toBeInTheDocument();

  const approvebutton=screen.getByTestId('approvebutton');
  expect(approvebutton).toBeInTheDocument();
  fireEvent.click(approvebutton)
});
