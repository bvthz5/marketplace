import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import WishlistIcon from "../WishlistIcon";
import { rest, server } from "../../../../../../testServer";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

window.scrollTo = jest.fn();

server.use(
  rest.post("https://localhost:8080/api/WishList", (req, res, ctx) => {
    return res(ctx.status(200));
  })
);

describe("wishlisticon", () => {
  test("should render wishlist icon and while loggedout set favotite as false", async () => {
    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = await screen.findByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();
  });

  test("should render wishlist icon and set favoutite as true", async () => {
    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={true} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);

    const loginButton = await screen.findByText(/Login Now/i);
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  test("should render wishlist icon and click cancel on login prompt", async () => {
    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={true} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);

    const cancelButton = await screen.findByText(/Cancel/i);
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);
  });

  test("add to wishlist success", async () => {
    server.use(
      rest.post("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );
    localStorage.setItem("accessToken", "testtoken");

    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={false} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);
  });

  test("add to wishlist error", async () => {
    server.use(
      rest.post("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    localStorage.setItem("accessToken", "testtoken");

    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={false} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);
  });

  test("remove from wishlist success response", async () => {
    server.use(
      rest.delete("https://localhost:8080/api/WishList/5", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );
    localStorage.setItem("accessToken", "testtoken");

    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={true} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);
  });

  test("remove from wishlist error response", async () => {
    server.use(
      rest.delete("https://localhost:8080/api/WishList/5", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    localStorage.setItem("accessToken", "testtoken");

    render(
      <Provider store={store}>
        <Router>
          <WishlistIcon id={5} favourite={true} />
        </Router>
      </Provider>
    );
    const wishlistIconElement = screen.getByTestId("wishlisticon5");
    expect(wishlistIconElement).toBeDefined();

    fireEvent.click(wishlistIconElement);
  });
});
