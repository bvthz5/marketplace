import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import OrderHistory from "../OrderHistory";
import { cartCounts } from "../../../../../App";
import { server, rest } from "../../../../../testServer";
import {
  orderHistoryData,
  orderHistoryDataWithoutImage,
} from "../testData/data";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

jest.mock("./../../../Assets/images/Image_not_available.png");
window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Order/list", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(orderHistoryData));
  })
);

const initialCartCount = 0;
const OrderHistoryWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <OrderHistory />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render orderhistory component with success response", async () => {
  render(<OrderHistoryWrapper />);
  const orderHistoryElement = screen.getByTestId("orderhistorypage");
  expect(orderHistoryElement).toBeDefined();

  const data = await screen.findByText(/101/i);
  expect(data).toBeInTheDocument();

  const navigatebtn = await screen.findByTestId("gotodetailbtn");
  fireEvent.click(navigatebtn);
});

test("should render orderhistory component with success response without thumbnail", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Order/list", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(orderHistoryDataWithoutImage));
    })
  );
  render(<OrderHistoryWrapper />);

  const orderHistoryElement = await screen.findByTestId("orderhistorypage");
  expect(orderHistoryElement).toBeDefined();

  const navigatebtn = await screen.findByTestId("gotodetailbtn");
  fireEvent.click(navigatebtn);
});

test("should render orderhistory component with error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Order/list", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(<OrderHistoryWrapper />);
  const orderHistoryElement = screen.getByTestId("orderhistorypage");
  expect(orderHistoryElement).toBeDefined();

  const noOrdersElement = await screen.findByText(/No Orders Found!/i);
  expect(noOrdersElement).toBeInTheDocument();
});
