import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import { cartCounts } from "../../../../../../App";
import Checkout from "../Checkout";
import { createOrderResponse, multipleAddressListData } from "../testData/data";
import { rest, server } from "../../../../../../testServer";
import { getProductDetailsData } from "../../../../Productlist/ProductDetailView/testData/data";
import { JSDOM } from "jsdom";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

// ==================== test for checkout page while there is a product id ====================
// ==================== product id is recieved when user clicks buy now button from order detail page ====================

window.scrollTo = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ product_id: "84" })],
}));

const initialCartCount = 0;
const CheckoutWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Checkout />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render checkout component with product id and get success response and click continue button then cancel the order", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Product/84", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailsData));
    })
  );

  server.use(
    rest.get("https://localhost:8080/api/delivery-address", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(multipleAddressListData));
    })
  );

  server.use(
    rest.post("https://localhost:8080/api/Order", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(createOrderResponse));
    })
  );

  server.use(
    rest.put(
      "https://localhost:8080/api/Order/cancel?orderNumber=order",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json({ status: true }));
      }
    )
  );

  localStorage.setItem("orderCreated", true);
  render(<CheckoutWrapper />);

  const checkoutElement = screen.getByTestId("checkoutpage");
  expect(checkoutElement).toBeDefined();
  const name = await screen.findByText(/Arun George/i);
  expect(name).toBeInTheDocument();

  const deliverHereBtn = await screen.findByTestId("deliverHere");
  expect(deliverHereBtn).toBeInTheDocument();

  fireEvent.click(deliverHereBtn);

  const continueButton = await screen.findByTestId("buynow-btn");
  expect(continueButton).toBeInTheDocument();

  fireEvent.click(continueButton);

  const cancelOrder = await screen.findByTestId("cancel-btn");
  expect(cancelOrder).toBeInTheDocument();

  fireEvent.click(cancelOrder);

  const swalConfirmation = await screen.findByText(/Yes/i);
  expect(swalConfirmation).toBeInTheDocument();

  fireEvent.click(swalConfirmation);
});

test("should render checkout component with product id and get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Product/84", (req, res, ctx) => {
      return res(ctx.status(400), ctx.json(getProductDetailsData));
    })
  );

  server.use(
    rest.get("https://localhost:8080/api/delivery-address", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(multipleAddressListData));
    })
  );

  server.use(
    rest.put(
      "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );

  localStorage.setItem("orderCreated", true);

  render(<CheckoutWrapper />);

  const checkoutElement = screen.getByTestId("checkoutpage");
  expect(checkoutElement).toBeDefined();
  const name = await screen.findByText(/Arun George/i);
  expect(name).toBeInTheDocument();

  const checkBox = await screen.findByTestId("checkbox-2");
  expect(checkBox).toBeInTheDocument();

  fireEvent.click(checkBox);

  const addAddressButton = await screen.findByTestId("add-address-button");
  expect(addAddressButton).toBeInTheDocument();

  fireEvent.click(addAddressButton);

  const deliverHereBtn = await screen.findByTestId("deliverHere");
  expect(deliverHereBtn).toBeInTheDocument();

  fireEvent.click(deliverHereBtn);

  const noProductFound = await screen.findByText(/No Products found/i);
  expect(noProductFound).toBeInTheDocument();

  const cancelOrder = await screen.findByTestId("cancel-btn");
  expect(cancelOrder).toBeInTheDocument();

  fireEvent.click(cancelOrder);

  const swalConfirmation = await screen.findByText(/Yes/i);
  expect(swalConfirmation).toBeInTheDocument();

  fireEvent.click(swalConfirmation);
});

test("should render checkout component and reload to get an alert to changes that are not saved", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Product/84", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getProductDetailsData));
    })
  );

  server.use(
    rest.get("https://localhost:8080/api/delivery-address", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(multipleAddressListData));
    })
  );

  localStorage.setItem("orderCreated", true);

  const dom = new JSDOM();
  global.window = dom.window;

  render(<CheckoutWrapper />);

  const checkoutElement = screen.getByTestId("checkoutpage");
  expect(checkoutElement).toBeDefined();
  const name = await screen.findByText(/Arun George/i);
  expect(name).toBeInTheDocument();

  const event = new Event("beforeunload");
  window.dispatchEvent(event);
});
