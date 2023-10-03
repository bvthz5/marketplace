import React, { useMemo, useState } from "react";
import { render, fireEvent, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import MyCart from "../MyCart";
import { cartCounts } from "../../../../App";
import { server, rest } from "../../../../testServer";
import {
  buyableData,
  deleteCartResponse,
  getCartData,
  getCartUnavailableData,
  nonBuyableData,
} from "../testData/data";
import { Provider } from "react-redux";
import { store } from "../../../../redux/store";

jest.mock("../../../../Assets/images/Image_not_available.png");
window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getCartData));
  })
);

const initialCartCount = 0;
const MyCartWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <MyCart />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render mycart component and get success response and click moredetails", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCartData));
    })
  );
  render(<MyCartWrapper />);

  const myCartElement = screen.getByTestId("mycartpage");
  expect(myCartElement).toBeDefined();

  const datacard = await screen.findByTestId("productcard");
  expect(datacard).toBeInTheDocument();

  const data = await screen.findByText(/Laptop/i);
  expect(data).toBeInTheDocument();

  // navigation test
  const navigatebtn = screen.getByTestId("navigatebtn");
  expect(navigatebtn).toBeInTheDocument();
  fireEvent.click(navigatebtn);
});

test("should render mycart component and get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );

  render(<MyCartWrapper />);

  const noProductsElement = await screen.findByText(/No Products Found!/i);
  expect(noProductsElement).toBeInTheDocument();
});

test("should render mycart component and get success response and the product is unavailable and also get succees on delete from cart", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCartUnavailableData));
    })
  );

  server.use(
    rest.delete("https://localhost:8080/api/Cart/84", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(deleteCartResponse));
    })
  );

  render(<MyCartWrapper />);

  const myCartElement = screen.getByTestId("mycartpage");
  expect(myCartElement).toBeDefined();

  const datacard = await screen.findByTestId("productcard");
  expect(datacard).toBeInTheDocument();

  const data = await screen.findByText(/Laptop/i);
  expect(data).toBeInTheDocument();

  const deletebtn = screen.getByTestId("deletebtn");
  expect(deletebtn).toBeInTheDocument();
  fireEvent.click(deletebtn);
});

test("should render mycart component and get success response and the product is unavailable and also get error on delete from cart", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCartUnavailableData));
    })
  );

  server.use(
    rest.delete("https://localhost:8080/api/Cart/84", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );

  render(<MyCartWrapper />);

  const myCartElement = screen.getByTestId("mycartpage");
  expect(myCartElement).toBeDefined();

  const datacard = await screen.findByTestId("productcard");
  expect(datacard).toBeInTheDocument();

  const data = await screen.findByText(/Laptop/i);
  expect(data).toBeInTheDocument();

  const deletebtn = screen.getByTestId("deletebtn");
  expect(deletebtn).toBeInTheDocument();
  fireEvent.click(deletebtn);
});

test("should render mycart component and and able to place order", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(buyableData));
    })
  );
  render(<MyCartWrapper />);

  const myCartElement = screen.getByTestId("mycartpage");
  expect(myCartElement).toBeDefined();

  const placeOrderBtn = await screen.findByTestId("placeorderbtn");
  expect(placeOrderBtn).toBeInTheDocument();
  fireEvent.click(placeOrderBtn);
});

test("should render mycart component and and not able to place order", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(nonBuyableData));
    })
  );
  render(<MyCartWrapper />);

  const myCartElement = screen.getByTestId("mycartpage");
  expect(myCartElement).toBeDefined();

  const placeOrderBtn = await screen.findByTestId("placeorderbtn");
  expect(placeOrderBtn).toBeInTheDocument();
  fireEvent.click(placeOrderBtn);
});
