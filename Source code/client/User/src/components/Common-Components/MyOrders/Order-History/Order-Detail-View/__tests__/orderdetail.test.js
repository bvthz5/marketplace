import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import { cartCounts } from "../../../../../../App";
import OrderDetailView from "../OrderDetailView";
import { server, rest } from "../../../../../../testServer";
import { orderDetailData } from "../testData/data";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

jest.mock("./../../../Assets/images/Image_not_available.png");
window.scrollTo = jest.fn();

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ orderDetailsId: "3" })],
}));

server.use(
  rest.get("https://localhost:8080/api/order/3", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(orderDetailData));
  })
);

const initialCartCount = 0;
const OrderDetaiWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);

  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <OrderDetailView />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render orderhistory component get success response", async () => {
  const pdfData = "hvedv";

  server.use(
    rest.get(
      "https://localhost:8080/api/order/download-invoice/3",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(pdfData));
      }
    )
  );

  server.use(
    rest.get(
      "https://localhost:8080/api/order/email-invoice/3",
      (req, res, ctx) => {
        return res(ctx.status(200));
      }
    )
  );

  render(<OrderDetaiWrapper />);
  const orderHistoryElement = screen.getByTestId("orderdetailpage");
  expect(orderHistoryElement).toBeDefined();

  const data = await screen.findByText(/101/i);
  expect(data).toBeInTheDocument();

  const downloadButton = await screen.findByTestId("downloadbtn");
  fireEvent.click(downloadButton);

  const emailButton = await screen.findByTestId("emailbtn");
  fireEvent.click(emailButton);

  const navigateButton = await screen.findByTestId("productname");
  fireEvent.click(navigateButton);
});

test("should render orderhistory click email button and download button and get get error response on both", async () => {
  server.use(
    rest.get("https://localhost:8080/api/order/3", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(orderDetailData));
    })
  );
  server.use(
    rest.get(
      "https://localhost:8080/api/order/download-invoice/3",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );

  server.use(
    rest.get(
      "https://localhost:8080/api/order/email-invoice/3",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );

  render(<OrderDetaiWrapper />);
  const orderHistoryElement = screen.getByTestId("orderdetailpage");
  expect(orderHistoryElement).toBeDefined();

  const downloadButton = await screen.findByTestId("downloadbtn");
  fireEvent.click(downloadButton);

  const emailButton = await screen.findByTestId("emailbtn");
  fireEvent.click(emailButton);

  const navigateButton = await screen.findByTestId("productname");
  fireEvent.click(navigateButton);
});

test("should render orderhistory component get suceess  response with order-status 7 and should redirected to orders page", async () => {
  server.use(
    rest.get("https://localhost:8080/api/order/3", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json({ data: { orderStatus: 6 } }));
    })
  );
  render(<OrderDetaiWrapper />);
  const orderHistoryElement = screen.getByTestId("orderdetailpage");
  expect(orderHistoryElement).toBeDefined();

  const data = await screen.findByTestId("detailsection");
  expect(data).toBeInTheDocument();
});

test("should render orderhistory component get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/order/3", (req, res, ctx) => {
      return res(ctx.status(404));
    })
  );
  render(<OrderDetaiWrapper />);
  const orderHistoryElement = screen.getByTestId("orderdetailpage");
  expect(orderHistoryElement).toBeDefined();

  const data = await screen.findByTestId("detailsection");
  expect(data).toBeInTheDocument();
});
