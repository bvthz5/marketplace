import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React, { useMemo, useState } from "react";
import { cartCounts } from "../../../../../App";
import DrawerData from "../DrawerData";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

window.scrollTo = jest.fn();

const initialCartCount = 0;
const DrawerDataWrapper = ({ toggleDrawer, setLoggedIn }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);

  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <DrawerData toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render drawercomponent", () => {
  const toggleDrawer = jest.fn();
  const setLoggedIn = jest.fn();
  render(
    <DrawerDataWrapper toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
  );
  const drawerDataElement = screen.getByTestId("drawercomponent");
  expect(drawerDataElement).toBeDefined();
});

test("renders first section of drawer", () => {
  const toggleDrawer = jest.fn();
  const setLoggedIn = jest.fn();
  render(
    <DrawerDataWrapper toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
  );
  const homeLink = screen.getByText(/Home/i);
  const cartLink = screen.getByText(/Cart/i);

  expect(homeLink).toBeInTheDocument();
  expect(cartLink).toBeInTheDocument();

  fireEvent.click(homeLink);
  fireEvent.click(cartLink);
});

test("renders second section of drawer", async () => {
  const toggleDrawer = jest.fn();
  const setLoggedIn = jest.fn();
  localStorage.setItem("role", "2");
  render(
    <DrawerDataWrapper toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
  );

  const myProfileLink = screen.getByText(/My Profile/i);
  const myAdsLink = screen.getByText(/My Ads/i);
  const wishlistLink = screen.getByText(/Wishlist/i);
  const myOrdersLink = screen.getByText(/My Orders/i);
  const logoutLink = screen.getByText(/Logout/i);

  expect(myProfileLink).toBeInTheDocument();
  expect(myAdsLink).toBeInTheDocument();
  expect(wishlistLink).toBeInTheDocument();
  expect(myOrdersLink).toBeInTheDocument();
  expect(logoutLink).toBeInTheDocument();

  fireEvent.click(myProfileLink);
  fireEvent.click(myAdsLink);
  fireEvent.click(wishlistLink);
  fireEvent.click(myOrdersLink);
  fireEvent.click(logoutLink);

  const logoutButton = await screen.findByText(/Yes, Logout!/i);
  expect(logoutButton).toBeInTheDocument();

  fireEvent.click(logoutButton);
});

test("renders second section of drawer and click logout and cancel the process", async () => {
  const toggleDrawer = jest.fn();
  const setLoggedIn = jest.fn();
  localStorage.setItem("role", "2");

  render(
    <DrawerDataWrapper toggleDrawer={toggleDrawer} setLoggedIn={setLoggedIn} />
  );

  const myProfileLink = screen.getByText(/My Profile/i);
  const myAdsLink = screen.getByText(/My Ads/i);
  const wishlistLink = screen.getByText(/Wishlist/i);
  const myOrdersLink = screen.getByText(/My Orders/i);
  const logoutLink = screen.getByText(/Logout/i);

  expect(myProfileLink).toBeInTheDocument();
  expect(myAdsLink).toBeInTheDocument();
  expect(wishlistLink).toBeInTheDocument();
  expect(myOrdersLink).toBeInTheDocument();
  expect(logoutLink).toBeInTheDocument();

  fireEvent.click(myProfileLink);
  fireEvent.click(myAdsLink);
  fireEvent.click(wishlistLink);
  fireEvent.click(myOrdersLink);
  fireEvent.click(logoutLink);

  const cancelButton = await screen.findByText(/Cancel/i);
  expect(cancelButton).toBeInTheDocument();

  fireEvent.click(cancelButton);
});
