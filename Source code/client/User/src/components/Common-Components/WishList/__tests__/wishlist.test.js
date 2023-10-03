import React, { useMemo, useState } from "react";
import { render, fireEvent, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import WishList from "../WishList";
import { cartCounts } from "../../../../App";
import { server, rest } from "../../../../testServer";
import {
  getWishlistData,
  getSingleWishlistData,
  deleteWishlistResponse,
} from "../testData/data";
import { store } from "../../../../redux/store";
import { Provider } from "react-redux";

jest.mock("../../../../Assets/images/Image_not_available.png");
window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getWishlistData));
  })
);

const initialCartCount = 0;
const WishListWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <WishList />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("wishlist", () => {
  test("should render wishlist component and call getwishlist and get success response", async () => {
    render(<WishListWrapper />);
    const wishlistElement = screen.getByTestId("wishlistpage");
    expect(wishlistElement).toBeDefined();
    const element = await screen.findByText(/29980/i);
    expect(element).toBeInTheDocument();
  });

  test("should render wishlist component and call getwishlist and get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(<WishListWrapper />);
    const wishlistElement = screen.getByTestId("wishlistpage");
    expect(wishlistElement).toBeDefined();
  });

  test("remove a product from wishlist when delete button is clicked get succeess response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleWishlistData));
      })
    );

    server.use(
      rest.delete("https://localhost:8080/api/WishList/60", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(deleteWishlistResponse));
      })
    );
    render(<WishListWrapper />);
    const element = await screen.findByText(/10000/i);
    expect(element).toBeInTheDocument();
    const deletebtn = screen.getByTestId("deletebtn");
    expect(deletebtn).toBeInTheDocument();
    fireEvent.click(deletebtn);
  });

  test("remove a product from wishlist when delete button is clicked get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleWishlistData));
      })
    );

    server.use(
      rest.delete("https://localhost:8080/api/WishList/60", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(<WishListWrapper />);
    const element = await screen.findByText(/10000/i);
    expect(element).toBeInTheDocument();
    const deletebtn = screen.getByTestId("deletebtn");
    expect(deletebtn).toBeInTheDocument();
    fireEvent.click(deletebtn);
  });

  test("navigate to detail when moredetails button is clicked", async () => {
    server.use(
      rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleWishlistData));
      })
    );

    render(<WishListWrapper />);
    const element = await screen.findByText(/10000/i);
    expect(element).toBeInTheDocument();
    const navigatebtn = screen.getByTestId("navigatebtn");
    expect(navigatebtn).toBeInTheDocument();
    fireEvent.click(navigatebtn);
  });
});
