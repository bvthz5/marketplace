import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import SellerDetail from '../SellerDetail';
import { rest, server } from '../../../../testServer';
import { getSellerDetailData } from '../testData/data';

jest.mock('./../../../assets/Screenshot-2018-12-16-at-21.06.29.png');
window.scrollTo = jest.fn();
describe('sellerdetail test', () => {
  test('should render Seller Detail component', async () => {
    server.use(
      rest.get('https://localhost:8080/api/User/14', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSellerDetailData));
      })
    );

    render(
      <Router>
        <SellerDetail seller={getSellerDetailData.data} />
      </Router>
    );
    const sellerDetailElement = screen.getByTestId('sellerdetailpage');
    expect(sellerDetailElement).toBeDefined();

    const avatar=screen.getByTestId("avatarid")
    expect(avatar).toBeInTheDocument();
    fireEvent.click(avatar);

    const emailuser = await screen.findByText('yecopat900@wiroute.com');
    expect(emailuser).toBeInTheDocument();
  });
  test('should render Seller Detail component with error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/User/14', (req, res, ctx) => {
        return res(ctx.status(400), ctx.json(getSellerDetailData));
      })
    );

    render(
      <Router>
           <SellerDetail seller={getSellerDetailData.data} />
      </Router>
    );
    const sellerDetailElement = screen.getByTestId('sellerdetailpage');
    expect(sellerDetailElement).toBeDefined();
  });
});
