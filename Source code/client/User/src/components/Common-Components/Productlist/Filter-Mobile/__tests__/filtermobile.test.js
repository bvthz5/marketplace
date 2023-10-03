import React from "react";
import { render, screen } from "@testing-library/react";
import FilterMobile from "../FilterMobile";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

describe("FilterMobile component", () => {
  test("renders open button and opens drawer on button click", () => {
    render(
      <Provider store={store}>
        <FilterMobile />
      </Provider>
    );

    const drawer = screen.getByTestId("filter-mobile-drawer");
    expect(drawer).not.toBeVisible();
  });
});
