import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React, { useMemo, useState } from "react";
import { cartCounts } from "../../../../../App";
import Header from "./../Header.jsx";
import { getCartData } from "../../../MyCart/testData/data";
import { server, rest } from "../../../../../testServer";
import { refreshData, sellerRequestRefreshData } from "../testData/data";
import userEvent from "@testing-library/user-event";
import { JSDOM } from "jsdom";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

jest.mock("../../../../../../node_modules/@react-google-maps/api", () => {
  return {
    useLoadScript: jest.fn().mockImplementation(() => {
      return {
        isLoaded: true,
        loadError: false,
      };
    }),
  };
});

jest.mock("react-places-autocomplete");

window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getCartData));
  })
);

server.use(
  rest.put(
    "https://localhost:8080/api/User/requset-to-seller",
    (req, res, ctx) => {
      return res(
        ctx.status(200),
        ctx.json({
          message: null,
          serviceStatus: 200,
          status: true,
        })
      );
    }
  )
);

server.use(
  rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(refreshData));
  })
);

const initialCartCount = 0;
const CommonHeaderWrapper = ({
  handleSearch,
  handleLocation,
  searchValue,
  selectedAddress,
}) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Header
            productlist={true}
            handleSearch={handleSearch}
            handleLocation={handleLocation}
            selectedAddress={selectedAddress}
            searchValue={searchValue}
          />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("test header component", () => {
  test("should render commonheader component", async () => {
    localStorage.clear();

    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    const cartButton = await screen.findByTestId("cartbtn");
    expect(cartButton).toBeInTheDocument();
    fireEvent.click(cartButton);

    const swalLoginButton1 = await screen.findByText(/Login Now/i);
    expect(swalLoginButton1).toBeInTheDocument();
    fireEvent.click(swalLoginButton1);

    const becomeSellerButton = await screen.findByTestId("becomesellerbtn");
    expect(becomeSellerButton).toBeInTheDocument();
    fireEvent.click(becomeSellerButton);

    const swalLoginButton2 = await screen.findByText(/Login Now/i);
    expect(swalLoginButton2).toBeInTheDocument();
    fireEvent.click(swalLoginButton2);

    const title = await screen.findByTestId("title");
    expect(title).toBeInTheDocument();
    fireEvent.click(title);

    const homeIcon = await screen.findByTestId("homeicon");
    expect(homeIcon).toBeInTheDocument();
    fireEvent.click(homeIcon);

    const loginButton = await screen.findByTestId("loginbtn");
    expect(loginButton).toBeInTheDocument();
    fireEvent.click(loginButton);

    const notificationButton = screen.getByTestId("notificationboxbtn");
    expect(notificationButton).toBeInTheDocument();
    fireEvent.click(notificationButton);
  });

  test("should render commonheader component and click cart icon", async () => {
    localStorage.clear();

    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    const cartButton = await screen.findByTestId("cartbtn");
    expect(cartButton).toBeInTheDocument();
    fireEvent.click(cartButton);

    const cancelButton1 = await screen.findByText(/Cancel/i);
    expect(cancelButton1).toBeInTheDocument();

    fireEvent.click(cancelButton1);

    const becomeSellerButton = await screen.findByTestId("becomesellerbtn");
    expect(becomeSellerButton).toBeInTheDocument();
    fireEvent.click(becomeSellerButton);

    const cancelButton2 = await screen.findByText(/Cancel/i);
    expect(cancelButton2).toBeInTheDocument();

    fireEvent.click(cancelButton2);

    const title = await screen.findByTestId("title");
    expect(title).toBeInTheDocument();
    fireEvent.click(title);

    const homeIcon = await screen.findByTestId("homeicon");
    expect(homeIcon).toBeInTheDocument();
    fireEvent.click(homeIcon);

    const loginButton = await screen.findByTestId("loginbtn");
    expect(loginButton).toBeInTheDocument();
    fireEvent.click(loginButton);
  });

  test("should render commonheader component and user should be logged in", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(sellerRequestRefreshData));
      })
    );
    server.use(
      rest.put(
        "https://localhost:8080/api/User/requset-to-seller",
        (req, res, ctx) => {
          return res(ctx.status(200));
        }
      )
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "0");
    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    const cartButton = await screen.findByTestId("cartbtn");
    expect(cartButton).toBeInTheDocument();
    fireEvent.click(cartButton);

    // opening modal
    const becomeSellerButton = await screen.findByTestId("becomesellerbtn");
    expect(becomeSellerButton).toBeInTheDocument();
    fireEvent.click(becomeSellerButton);

    const termsHeading = await screen.findByTestId("modalheading");
    expect(termsHeading).toBeInTheDocument();

    // closing modal
    const disagreeButton = await screen.findByTestId("disagree-button");
    expect(disagreeButton).toBeInTheDocument();
    fireEvent.click(disagreeButton);

    // again opening modal
    fireEvent.click(becomeSellerButton);
    expect(termsHeading).toBeInTheDocument();

    const checkButton = await screen.findByTestId("checkbox");
    expect(checkButton).toBeInTheDocument();
    fireEvent.click(checkButton);

    const agreeButton = await screen.findByTestId("agreebutton");
    expect(agreeButton).toBeInTheDocument();
    fireEvent.click(agreeButton);

    // repeating again to force ui to throw error as request already submitted
    localStorage.setItem("role", "1");

    fireEvent.click(becomeSellerButton);

    expect(termsHeading).toBeInTheDocument();

    fireEvent.click(checkButton);

    fireEvent.click(agreeButton);
  });

  test("get error response on req as seller", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(sellerRequestRefreshData));
      })
    );
    server.use(
      rest.put(
        "https://localhost:8080/api/User/requset-to-seller",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "0");
    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    // opening modal
    const becomeSellerButton = await screen.findByTestId("becomesellerbtn");
    expect(becomeSellerButton).toBeInTheDocument();
    fireEvent.click(becomeSellerButton);

    const termsHeading = await screen.findByTestId("modalheading");
    expect(termsHeading).toBeInTheDocument();

    const checkButton = await screen.findByTestId("checkbox");
    expect(checkButton).toBeInTheDocument();
    fireEvent.click(checkButton);

    const agreeButton = await screen.findByTestId("agreebutton");
    expect(agreeButton).toBeInTheDocument();
    fireEvent.click(agreeButton);
  });

  test("user should be logged in and click become a seller button", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(refreshData));
      })
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "1");

    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    const becomeSellerButton = await screen.findByTestId("becomesellerbtn");
    expect(becomeSellerButton).toBeInTheDocument();
    fireEvent.click(becomeSellerButton);
  });

  test("should render commonheader component and user should be logged in and click sell button", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(refreshData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/User/requset-to-seller`",
        (req, res, ctx) => {
          return res(ctx.status(200));
        }
      )
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "2");

    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    const sellButton = await screen.findByTestId("sellbtn");
    expect(sellButton).toBeInTheDocument();
    fireEvent.click(sellButton);
  });

  test("user should be logged in and get error response on cart list call and refresh call error", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "1");

    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();
  });

  test("enter value to search field", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(sellerRequestRefreshData));
      })
    );

    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testtoken");
    localStorage.setItem("role", "1");
    const user = userEvent.setup();

    const dom = new JSDOM();
    global.window = dom.window;
    render(<CommonHeaderWrapper />);
    const commonHeaderElement = screen.getByTestId("commonheader");
    expect(commonHeaderElement).toBeDefined();

    await user.type(screen.getByTestId(/search-input/i), "apple");
    expect(screen.getByTestId(/search-input/i)).toHaveValue("apple");

    const searchButton = await screen.findByTestId("search-btn");
    expect(searchButton).toBeInTheDocument();
    fireEvent.click(searchButton);

    window.innerWidth = 250;
    window.dispatchEvent(new Event("resize"));

    const menuIcon = await screen.findByTestId("menu-icon");
    expect(menuIcon).toBeInTheDocument();
    fireEvent.click(menuIcon);

    fireEvent.click(searchButton);
  });
});
