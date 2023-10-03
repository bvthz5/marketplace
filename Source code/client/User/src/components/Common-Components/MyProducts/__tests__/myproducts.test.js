import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import MyProducts from "../MyProducts";
import { cartCounts } from "../../../../App";
import {
  myProductsData,
  status_0,
  status_1,
  status_3,
  status_4,
  status_5,
  status_6,
} from "../testData/data";
import { server, rest } from "../../../../testServer";
import useWindowDimensions from "../../../../hooks/WindowSizeReader/WindowDimensions";
import { Provider } from "react-redux";
import { store } from "../../../../redux/store";

window.scrollTo = jest.fn();

jest.mock("../../../../hooks/WindowSizeReader/WindowDimensions", () => ({
  __esModule: true,
  default: jest.fn().mockReturnValue({ width: 800, height: 600 }),
}));

server.use(
  rest.get("https://localhost:8080/api/Product/by-user/0", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(myProductsData));
  })
);

const initialCartCount = 0;
const MyProductsWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <MyProducts />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render myproducts component and gdelet product and get success response", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  server.use(
    rest.put(
      "https://localhost:8080/api/product/delete/91",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  const deletebutton = await screen.findByTestId("deletebtn");
  fireEvent.click(deletebutton);

  const swalDeleteButton = await screen.findByText(/Yes, delete it!/i);
  expect(swalDeleteButton).toBeInTheDocument();
  fireEvent.click(swalDeleteButton);
});

test("should render myproducts component and delete product and get success response", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  server.use(
    rest.put(
      "https://localhost:8080/api/product/delete/91",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  const deletebutton = await screen.findByTestId("deletebtn");
  fireEvent.click(deletebutton);

  const cancelButton = await screen.findByText(/Cancel/i);
  expect(cancelButton).toBeInTheDocument();

  fireEvent.click(cancelButton);
});

test("should render myproducts component and delete product and confirm on swal and get error response and click image edit btn", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  server.use(
    rest.put(
      "https://localhost:8080/api/product/delete/91",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );
  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  const deletebutton = await screen.findByTestId("deletebtn");
  fireEvent.click(deletebutton);

  const swalDeleteButton = await screen.findByText(/Yes, delete it!/i);
  expect(swalDeleteButton).toBeInTheDocument();
  fireEvent.click(swalDeleteButton);

  const editButton = await screen.findByTestId("editimagebtn");
  fireEvent.click(editButton);
});

test("should render myproducts component and delete product and cancel swal and get error response and click image edit btn", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );
  server.use(
    rest.put(
      "https://localhost:8080/api/product/delete/91",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );
  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  const deletebutton = await screen.findByTestId("deletebtn");
  fireEvent.click(deletebutton);

  const cancelButton = await screen.findByText(/Cancel/i);
  expect(cancelButton).toBeInTheDocument();

  fireEvent.click(cancelButton);

  const editButton = await screen.findByTestId("editimagebtn");
  fireEvent.click(editButton);
});

test("should render myproducts component and click details edit btn", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(myProductsData));
      }
    )
  );

  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  expect(await screen.findByText(/Pending for approval/i)).toBeInTheDocument();

  const editButton = await screen.findByTestId("editdetailsbtn");
  fireEvent.click(editButton);
});

test("should render myproducts component and get error response", async () => {
  server.use(
    rest.get(
      "https://localhost:8080/api/Product/by-user/0",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );
  render(<MyProductsWrapper />);
  const myProductsElement = screen.getByTestId("myproductspage");
  expect(myProductsElement).toBeDefined();

  const noProductsElement = await screen.findByText(/No Products Found!/i);
  expect(noProductsElement).toBeInTheDocument();
});

describe("render component with diffrent product status to show diffrent product status", () => {
  test("Rejected", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_0));
        }
      )
    );

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/Rejected/i)).toBeInTheDocument();
  });
  test("Approved", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_1));
        }
      )
    );

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/Approved/i)).toBeInTheDocument();
  });
  test("Deleted", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_4));
        }
      )
    );

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/Deleted/i)).toBeInTheDocument();
  });
  test("draft", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_5));
        }
      )
    );

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/draft/i)).toBeInTheDocument();
  });
  test("Order Processing", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_6));
        }
      )
    );
    useWindowDimensions.mockReturnValue({ width: 300, height: 600 });

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/Order Processing/i)).toBeInTheDocument();
  });
  test("Sold", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/Product/by-user/0",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(status_3));
        }
      )
    );

    render(<MyProductsWrapper />);
    const myProductsElement = screen.getByTestId("myproductspage");
    expect(myProductsElement).toBeDefined();

    expect(await screen.findByText(/Sold/i)).toBeInTheDocument();
  });
});
