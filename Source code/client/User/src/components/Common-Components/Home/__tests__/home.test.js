import React, { useMemo, useState } from "react";
import { render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import { cartCounts } from "../../../../App";
import Home from "../Home";
import { Provider } from "react-redux";
import { store } from "../../../../redux/store";

jest.mock("../../../../Assets/images/banner_one.png");
jest.mock("../../../../Assets/images/banner_two.png");
jest.mock("../../../../Assets/images/banner_three.png");
window.scrollTo = jest.fn();

const initialCartCount = 0;
const HomeWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Home />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render productlist component", () => {
  render(<HomeWrapper />);
  const productListElement = screen.getByTestId("productlistpage");
  expect(productListElement).toBeDefined();
});
