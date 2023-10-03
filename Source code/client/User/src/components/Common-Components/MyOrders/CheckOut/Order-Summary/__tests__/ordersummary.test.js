import React, { useMemo, useState } from "react";
import { render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import OrderSummary from "../OrderSummary";
import { cartCounts } from "../../../../../../App";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

jest.mock("../../../../../Assets/images/Image_not_available.png");

const initialCartCount = 0;
const OrderSummaryWrapper = ({ cart }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <OrderSummary cart={cart} />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render ordersummary component with data", () => {
  const cart = [
    {
      createdUserStatus: 1,
      productId: 73,
      productName: "Apple 2022 12.9-inch iPad Pro",
      categoryId: 14,
      categoryName: "Ipad",
      thumbnail: "73_dfcd7a78-88c0-4133-a380-9eb7b0480b89.jpg",
      productDescription: "Brilliant 12.9-inch",
      address: "Calicut, Kerala",
      price: 101,
      createdDate: "2023-02-23T17:08:07.8383145",
      status: 1,
    },
  ];
  render(<OrderSummaryWrapper cart={cart} />);
  const orderSummaryElement = screen.getByTestId("ordersummary");
  expect(orderSummaryElement).toBeDefined();
});

test("should render ordersummary component with data but no product image", () => {
  const cart = [
    {
      createdUserStatus: 1,
      productId: 73,
      productName: "Apple 2022 12.9-inch iPad Pro",
      categoryId: 14,
      categoryName: "Ipad",
      thumbnail: null,
      productDescription: "Brilliant 12.9-inch",
      address: "Calicut, Kerala",
      price: 101,
      createdDate: "2023-02-23T17:08:07.8383145",
      status: 1,
    },
  ];
  render(<OrderSummaryWrapper cart={cart} />);
  const orderSummaryElement = screen.getByTestId("ordersummary");
  expect(orderSummaryElement).toBeDefined();
});

test("should render ordersummary component without data", async () => {
  const cart = [];
  render(<OrderSummaryWrapper cart={cart} />);
  const orderSummaryElement = screen.getByTestId("ordersummary");
  expect(orderSummaryElement).toBeDefined();

  const noProductsElement = await screen.findByText(/No Products found/i);
  expect(noProductsElement).toBeInTheDocument();
});
