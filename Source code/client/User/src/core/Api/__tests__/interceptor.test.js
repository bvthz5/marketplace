import React, { useMemo, useState } from "react";
import { render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import { rest, server } from "../../../testServer";
import { cartCounts } from "../../../App";
import MyCart from "../../../components/Common-Components/MyCart/MyCart";
import { userLoginData } from "../../login/login-page/testData/data";
import { store } from "../../../redux/store";
import { Provider } from "react-redux";

jest.mock("../../../../Assets/images/Image_not_available.png");
window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
    return res(ctx.status(401));
  })
);

const initialCartCount = 0;
const MyCartWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  localStorage.setItem("accessToken", "testtoken");
  localStorage.setItem("refreshToken", "testrefreshtoken");
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

describe("test for covering interceptor", () => {
  test("pass 401 error to make interceptor to call refresh api and get success response", async () => {
    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userLoginData));
      })
    );
    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testrefreshtoken");

    render(<MyCartWrapper />);

    const myCartElement = screen.getByTestId("mycartpage");
    expect(myCartElement).toBeDefined();
  });

  test("pass 401 error to make interceptor to call refresh api and get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(401));
      })
    );
    server.use(
      rest.put("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    localStorage.setItem("accessToken", "testtoken");
    localStorage.setItem("refreshToken", "testrefreshtoken");

    render(<MyCartWrapper />);

    const myCartElement = screen.getByTestId("mycartpage");
    expect(myCartElement).toBeDefined();
  });
});
