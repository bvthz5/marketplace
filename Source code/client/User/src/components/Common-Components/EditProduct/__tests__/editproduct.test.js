import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import EditProduct from "../EditProduct";
import { cartCounts } from "../../../../App";
import { rest, server } from "../../../../testServer";
import { categoryData, editProductDetailsData } from "../testData/data";
import userEvent from "@testing-library/user-event";
import { store } from "../../../../redux/store";
import { Provider } from "react-redux";

window.scrollTo = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ id: "28" })],
}));

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
server.use(
  rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(editProductDetailsData));
  })
);

const initialCartCount = 0;
const EditProductWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);

  const match = { params: { id: 3 } };
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <EditProduct match={match} />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};
describe("edit product details", () => {
  test("enter invalid values to the input fields", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(editProductDetailsData));
      })
    );
    const user = userEvent.setup();

    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();

    await user.type(screen.getByTestId(/productname-input/i), "~");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("Beats~");

    expect(screen.getByTestId(/description-input/i)).toHaveValue("Custom");
    await user.type(screen.getByTestId(/description-input/i), "s");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");
    userEvent.clear(screen.getByTestId(/description-input/i));

    expect(screen.getByTestId(/price-input/i)).toHaveValue("11069");
    await user.type(screen.getByTestId(/price-input/i), "00");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("1106900");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("enter valid values to the input fields and clear them", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(editProductDetailsData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );
    const user = userEvent.setup();

    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();

    await user.type(screen.getByTestId(/productname-input/i), "s");
    userEvent.clear(screen.getByTestId(/productname-input/i));
    await user.type(screen.getByTestId(/productname-input/i), "Beats");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("Beats");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("enter valid values to the input fields and submit to get success response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(editProductDetailsData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );
    const user = userEvent.setup();

    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();

    await user.type(screen.getByTestId(/productname-input/i), "s");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("Beatss");

    expect(screen.getByTestId(/description-input/i)).toHaveValue("Custom");
    await user.type(screen.getByTestId(/description-input/i), "s");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");

    expect(screen.getByTestId(/price-input/i)).toHaveValue("11069");
    await user.type(screen.getByTestId(/price-input/i), "0");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("110690");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("enter valid values to the input fields and submit to get error response(Product Not Found)", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(editProductDetailsData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "Product Not Found" }));
      })
    );
    const user = userEvent.setup();

    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();

    await user.type(screen.getByTestId(/productname-input/i), "s");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("Beatss");

    expect(screen.getByTestId(/description-input/i)).toHaveValue("Custom");
    await user.type(screen.getByTestId(/description-input/i), "s");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");

    expect(screen.getByTestId(/price-input/i)).toHaveValue("11069");
    await user.type(screen.getByTestId(/price-input/i), "0");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("110690");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("enter valid values to the input fields and submit to get error response(Unhandled message)", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(editProductDetailsData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "Unhandled message" }));
      })
    );
    const user = userEvent.setup();

    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();

    await user.type(screen.getByTestId(/productname-input/i), "s");
    expect(screen.getByTestId(/productname-input/i)).toHaveValue("Beatss");

    expect(screen.getByTestId(/description-input/i)).toHaveValue("Custom");
    await user.type(screen.getByTestId(/description-input/i), "s");
    expect(screen.getByTestId(/description-input/i)).toHaveValue("Customs");

    expect(screen.getByTestId(/price-input/i)).toHaveValue("11069");
    await user.type(screen.getByTestId(/price-input/i), "0");
    expect(screen.getByTestId(/price-input/i)).toHaveValue("110690");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("should render editproduct component get success response on category list and error getProductdetails", () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );
    server.use(
      rest.get("https://localhost:8080/api/Product/28", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();
  });

  test("should render editproduct component get  error on categorylist", () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(<EditProductWrapper />);
    const editProductElement = screen.getByTestId("editproductpage");
    expect(editProductElement).toBeDefined();
  });
});
