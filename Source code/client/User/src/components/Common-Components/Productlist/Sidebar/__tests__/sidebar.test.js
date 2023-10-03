import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import React from "react";
import Sidebar from "../Sidebar";
import { rest, server } from "../../../../../testServer";
import { categoryData } from "../../../EditProduct/testData/data";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

server.use(
  rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(categoryData));
  })
);

describe("sidebar component", () => {
  test("should render sidebar component", () => {
    render(
      <Provider store={store}>
        <Sidebar />
      </Provider>
    );
    const sidebarElement = screen.getByTestId("sidebar");
    expect(sidebarElement).toBeDefined();

    const clearAllButton = screen.getByTestId("clear-all-button");
    expect(clearAllButton).toBeDefined();
    fireEvent.click(clearAllButton);
  });

  test("change category sort and price sort dropdowns", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryData));
      })
    );

    render(
      <Provider store={store}>
        <Sidebar />
      </Provider>
    );

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

    const clearAllButton = screen.getByTestId("clear-all-button");
    expect(clearAllButton).toBeDefined();
    fireEvent.click(clearAllButton);
  });

  test("error on category get api", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    render(
      <Provider store={store}>
        <Sidebar />
      </Provider>
    );

    const sidebarElement = screen.getByTestId("sidebar");
    expect(sidebarElement).toBeDefined();

    const applyButton = screen.getByTestId("apply-button");
    expect(applyButton).toBeDefined();
    fireEvent.click(applyButton);

  });
});
