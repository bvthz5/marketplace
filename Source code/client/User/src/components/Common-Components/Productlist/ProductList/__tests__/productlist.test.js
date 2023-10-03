import React, { useMemo, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import Productlist from "../Productlist";
import { cartCounts } from "../../../../../App";
import { rest, server } from "../../../../../testServer";
import { categoryData } from "../../../EditProduct/testData/data";
import { getCartData } from "../../../MyCart/testData/data";
import { productListData } from "../testData/data";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

jest.mock("../../../../../Assets/images/banner_one.png");
jest.mock("../../../../../Assets/images/banner_two.png");
jest.mock("../../../../../Assets/images/banner_three.png");
window.scrollTo = jest.fn();

server.use(
  rest.get(
    "https://localhost:8080/api/Product/page?Offset=0&PageSize=24&CategoryId=&Search=&Location=&StartPrice=0&EndPrice=0&SortBy=CreatedDate&SortByDesc=true",
    (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(productListData));
    }
  )
);

server.use(
  rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(categoryData));
  })
);

server.use(
  rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getCartData));
  })
);

const initialCartCount = 0;

const ProductlistWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Productlist />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("productlist component", () => {
  // ================================================ use filters
  test("render productlist and apply diffrent filters", async () => {
    localStorage.clear();

    const user = userEvent.setup();

    render(<ProductlistWrapper />);
    const productListElement = screen.getByTestId("productlistpage");
    expect(productListElement).toBeDefined();

    const data = await screen.findByText(/Apple/i);
    expect(data).toBeInTheDocument();

    const card = await screen.findByTestId("product-card");
    expect(card).toBeInTheDocument();

    fireEvent.click(card);

    const clearAllButton = screen.getByTestId("clear-all-button");
    expect(clearAllButton).toBeDefined();
    fireEvent.click(clearAllButton);

    // CHECK IF SELECT DROPDOWN EXISTS
    const priceSort = await waitFor(
      () => screen.findByTestId("price-sort-dropdown"),
      {
        timeout: 3000,
      }
    );
    expect(priceSort).toBeInTheDocument();

    fireEvent.change(priceSort, {
      target: { value: "Price:low to high" },
    });

    priceSort.dispatchEvent(new Event("change"));

    fireEvent.change(priceSort, {
      target: { value: "Price:high to low" },
    });

    priceSort.dispatchEvent(new Event("change"));
    fireEvent.change(priceSort, {
      target: { value: "Oldest to Newest" },
    });

    priceSort.dispatchEvent(new Event("change"));
    fireEvent.change(priceSort, {
      target: { value: "Newest to Oldest" },
    });

    priceSort.dispatchEvent(new Event("change"));

    const categorySort = await waitFor(
      () => screen.findByTestId("category-sort-dropdown"),
      {
        timeout: 3000,
      }
    );
    expect(categorySort).toBeInTheDocument();

    fireEvent.change(categorySort, {
      target: { value: "Camera" },
    });

    categorySort.dispatchEvent(new Event("change"));

    await user.type(screen.getByTestId(/search-input/i), "apple");
    expect(screen.getByTestId(/search-input/i)).toHaveValue("apple");
  });

  

  test("get success response while logged in get error response on getcart,wishlist", () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/page?Offset=0&PageSize=24&CategoryId=&Search=&Location=&StartPrice=0&EndPrice=0&SortBy=CreatedDate&SortByDesc=true",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(productListData));
        }
      )
    );
    server.use(
      rest.get("https://localhost:8080/api/WishList", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    localStorage.setItem("accessToken", "testtoken");
    render(<ProductlistWrapper />);
    const productListElement = screen.getByTestId("productlistpage");
    expect(productListElement).toBeDefined();


  });

  test("should render productlist component and get error response", () => {
    localStorage.clear();
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/page?Offset=0&PageSize=24&CategoryId=&Search=&Location=&StartPrice=0&EndPrice=0&SortBy=CreatedDate&SortByDesc=true",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );
    render(<ProductlistWrapper />);
    const productListElement = screen.getByTestId("productlistpage");
    expect(productListElement).toBeDefined();
  });
});
