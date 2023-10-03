import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import Addproduct from "../Addproduct";
import { cartCounts } from "../../../../App";
import userEvent from "@testing-library/user-event";
import { rest, server } from "../../../../testServer";
import { categoryData } from "../../EditProduct/testData/data";
import { store } from "../../../../redux/store";
import { Provider } from "react-redux";

window.scrollTo = jest.fn();

jest.mock("../../../../../node_modules/@react-google-maps/api", () => {
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

server.use(
  rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(categoryData));
  })
);

const initialCartCount = 0;
const AddproductWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Addproduct />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("add product", () => {
  test("should render addproduct component", async () => {
    const user = userEvent.setup();

    render(<AddproductWrapper />);
    const addProductElement = screen.getByTestId("addproductpage");
    expect(addProductElement).toBeDefined();

    fireEvent.click(screen.getByTestId("gobackbutton"));

    await user.type(screen.getByTestId(/productname-input/i), "apple");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("apple");

    await user.type(screen.getByTestId(/description-input/i), "Customs");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");

    await user.type(screen.getByTestId(/price-input/i), "11069");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("11069");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("enter invalid values in input fields", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    const user = userEvent.setup();

    render(<AddproductWrapper />);
    const addProductElement = screen.getByTestId("addproductpage");
    expect(addProductElement).toBeDefined();

    fireEvent.click(screen.getByTestId("gobackbutton"));

    await user.type(screen.getByTestId(/productname-input/i), "~");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("~");

    await user.type(screen.getByTestId(/description-input/i), "Customs");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");
    userEvent.clear(screen.getByTestId(/description-input/i));

    await user.type(screen.getByTestId(/price-input/i), "1106900");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("1106900");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });
});
