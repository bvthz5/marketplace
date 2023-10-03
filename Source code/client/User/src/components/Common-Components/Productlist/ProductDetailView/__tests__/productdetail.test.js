import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import ProductlistDetailView from "../ProductlistDetailView";
import { cartCounts } from "../../../../../App";
import { server, rest } from "../../../../../testServer";
import {
  cartDataForDetail,
  detailOfCartProduct,
  getProductDetailsData,
  getProductImagesData,
} from "../testData/data";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

window.matchMedia = jest.fn().mockImplementation((query) => {
  return {
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(),
    removeListener: jest.fn(),
  };
});

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ id: "84" })],
}));

jest.mock("../../../../../Assets/images/Screenshot-2018-12-16-at-21.06.29.png");
window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductDetailsData));
  })
);

server.use(
  rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductImagesData));
  })
);

const initialCartCount = 0;
const ProductlistDetailViewWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <ProductlistDetailView />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("product detailview", () => {
  test("should render detailview component get succeess response", async () => {
    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();
  });

  test("should render detailview component get failure response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(404));
      })
    );

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();
  });

  test("should render detailview component get succeess response with image api call error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductDetailsData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();
  });

  test("should render detailview component  with getcart  success response and able to add to cart and able  to buy", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductDetailsData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(cartDataForDetail));
      })
    );

    server.use(
      rest.post("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(
          ctx.status(200),
          ctx.json({
            data: null,
            message: "Product Added",
            serviceStatus: 200,
            status: true,
          })
        );
      })
    );
    localStorage.setItem("accessToken", "wvdjwydjwdjwfjdcxt");

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const addtoCartButton = await screen.findByTestId("addtocartbtn");
    expect(addtoCartButton).toBeInTheDocument();
    fireEvent.click(addtoCartButton);

    const buyNowButton = await screen.findByTestId("buynowbtn");
    expect(buyNowButton).toBeInTheDocument();
    fireEvent.click(buyNowButton);
  });

  test("should render detailview component  with getcart  success response and while adding to cart get error response (Cart Max Limit (50) exceed)", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductDetailsData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(cartDataForDetail));
      })
    );

    server.use(
      rest.post("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({
            data: null,
            message: "Cart Max Limit (50) exceed",
          })
        );
      })
    );
    localStorage.setItem("accessToken", "wvdjwydjwdjwfjdcxt");

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const addtoCartButton = await screen.findByTestId("addtocartbtn");
    expect(addtoCartButton).toBeInTheDocument();
    fireEvent.click(addtoCartButton);
  });

  test("should render detailview component  with getcart  success response and while adding to cart get error response (Unhandled Error Response)", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductDetailsData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(cartDataForDetail));
      })
    );

    server.use(
      rest.post("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({
            data: null,
            message: "Unhandled Error Response",
          })
        );
      })
    );
    localStorage.setItem("accessToken", "wvdjwydjwdjwfjdcxt");

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const addtoCartButton = await screen.findByTestId("addtocartbtn");
    expect(addtoCartButton).toBeInTheDocument();
    fireEvent.click(addtoCartButton);
  });

  test("should render detailview component  with getcart  success response and  not able to buy and click login now button login prompt", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(detailOfCartProduct));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    localStorage.clear();

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const buyNowButton = await screen.findByTestId("buynowbtn");
    expect(buyNowButton).toBeInTheDocument();
    fireEvent.click(buyNowButton);

    const loginButton1 = await screen.findByText(/Login Now/i);
    expect(loginButton1).toBeInTheDocument();

    fireEvent.click(loginButton1);

    const addtoCartButton = await screen.findByTestId("addtocartbtn");
    expect(addtoCartButton).toBeInTheDocument();
    fireEvent.click(addtoCartButton);

    const loginButton2 = await screen.findByText(/Login Now/i);
    expect(loginButton2).toBeInTheDocument();

    fireEvent.click(loginButton2);
  });

  test("should render detailview component  with getcart  success response and  not able to buy cancel login prompt", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(detailOfCartProduct));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    localStorage.clear();

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const buyNowButton = await screen.findByTestId("buynowbtn");
    expect(buyNowButton).toBeInTheDocument();
    fireEvent.click(buyNowButton);

    const cancelButton1 = await screen.findByText(/Cancel/i);
    expect(cancelButton1).toBeInTheDocument();

    fireEvent.click(cancelButton1);

    const addtoCartButton = await screen.findByTestId("addtocartbtn");
    expect(addtoCartButton).toBeInTheDocument();
    fireEvent.click(addtoCartButton);

    const cancelButton2 = await screen.findByText(/Cancel/i);
    expect(cancelButton2).toBeInTheDocument();

    fireEvent.click(cancelButton2);
  });

  test("should render detailview component with getcart  error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductDetailsData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    localStorage.setItem("accessToken", "wvdjwydjwdjwfjdcxt");
    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();
  });

  test("should render detailview component  with getcart  success response and should go to cart when go to cart button is clicked", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Product/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(detailOfCartProduct));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Photos/0", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductImagesData));
      })
    );

    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(cartDataForDetail));
      })
    );

    localStorage.setItem("accessToken", "wvdjwydjwdjwfjdcxt");

    render(<ProductlistDetailViewWrapper />);
    const detailViewElement = screen.getByTestId("detailviewpage");
    expect(detailViewElement).toBeDefined();

    const goToCartButton = await screen.findByTestId("gotocartbtn");
    expect(goToCartButton).toBeInTheDocument();
    fireEvent.click(goToCartButton);
  });
});
