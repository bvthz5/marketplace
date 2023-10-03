import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import AdminProductDetails from '../AdminProductDetailView';
import { rest, server } from '../../../../testServer';
import { getProductDetailData, pendingProductData, rejectedProductData } from '../testData/data';
import { getProductImagesData } from '../testData/data';
import userEvent from '@testing-library/user-event';

jest.mock('./../../assets/Screenshot-2018-12-16-at-21.06.29.png');
window.scrollTo = jest.fn();

jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useSearchParams: () => [new URLSearchParams({ id: '8' })],
}));

const setData = async () => {
  const product = userEvent.setup();
  await product.type(screen.getByTestId('productname-input'), 's');
  expect(screen.getByTestId('productname-input')).toHaveValue('Noises');
};

server.use(
  rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductDetailData));
  })
);
server.use(
  rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductImagesData));
  })
);

describe('productlist component', () => {
test('should render Adminproductdetail component', () => {
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const adminProductDetailsElement = screen.getByTestId('adminproductdetailspage');
  expect(adminProductDetailsElement).toBeDefined();
});

test('should render detailview component get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );

  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const nextImageButton = await screen.findByTestId('nextimagebtn');
  fireEvent.click(nextImageButton);
  fireEvent.click(nextImageButton);
  fireEvent.click(nextImageButton);

  const prevImageButton = await screen.findByTestId('previmagebtn');
  fireEvent.click(prevImageButton);
  fireEvent.click(prevImageButton);
  fireEvent.click(prevImageButton);

  const imageTag = await screen.findByTestId('imagetag');
  fireEvent.click(imageTag);

  const fullScreenImage = await screen.findByTestId('fullscreenimage');

  fireEvent.keyDown(fullScreenImage, { keyCode: 27 });
});
test('should render detailview component get failure response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(404));
    })
  );

  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();
});

test('should render detailview component get succeess response with image api call error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();
});
test('Can edit product name for approved products', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const editIcon = await screen.findByTestId('editIcon');
  expect(editIcon).toBeInTheDocument();
  fireEvent.click(editIcon);
  const editnamemodal = await screen.findByText('Edit Product Name');
  expect(editnamemodal).toBeInTheDocument();
  const user = userEvent.setup();
 
  await setData();

  const editsubmitbutton = await screen.findByTestId('editsubmitbutton');

  expect(editsubmitbutton).toBeInTheDocument();
  fireEvent.click(editsubmitbutton);



});

test('Approve button will approve product', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(pendingProductData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const approvebutton = await screen.findByTestId('approvebutton');
  expect(approvebutton).toBeInTheDocument();
  fireEvent.click(approvebutton);

  const confirmSwal = await screen.findByText(/Yes, Approve it!/i);
  expect(confirmSwal).toBeInTheDocument();
  fireEvent.click(confirmSwal);
});
test('cancel button in approve swal', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(pendingProductData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const approvebutton = await screen.findByTestId('approvebutton');
  expect(approvebutton).toBeInTheDocument();
  fireEvent.click(approvebutton);

  const cancelbutton = await screen.findByText(/Cancel/i);
  expect(cancelbutton).toBeInTheDocument();
  fireEvent.click(cancelbutton);
});
test('product reject button click  popup a modal', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(pendingProductData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const rejectButton = await screen.findByTestId('rejectButton');
  expect(rejectButton).toBeInTheDocument();
  fireEvent.click(rejectButton);

  const rejectModal = await screen.findByText(/Reject Request/i);
  expect(rejectModal).toBeInTheDocument();
  fireEvent.click(rejectModal);

  const product = userEvent.setup();
  await product.type(screen.getByTestId('reason-input'), 'ssdddddd');
  expect(screen.getByTestId('reason-input')).toHaveValue('ssdddddd');

  const submitButton = await screen.findByTestId('submitButton');
  expect(submitButton).toBeInTheDocument();
  fireEvent.click(submitButton);
});
test('cancel button click on reason modal', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Product/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(pendingProductData));
    })
  );
  server.use(
    rest.get('https://localhost:8080/api/Photos/0', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductImagesData));
    })
  );
  render(
    <Router>
      <AdminProductDetails />
    </Router>
  );
  const detailViewElement = screen.getByTestId('adminproductdetailspage');
  expect(detailViewElement).toBeDefined();

  const rejectButton = await screen.findByTestId('rejectButton');
  expect(rejectButton).toBeInTheDocument();
  fireEvent.click(rejectButton);

  const cancelReasonButton = await screen.findByText(/Cancel/i);
  expect(cancelReasonButton).toBeInTheDocument();
  fireEvent.click(cancelReasonButton);
});
});