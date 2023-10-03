import { render, screen } from "@testing-library/react";
import App from "../App";

import React from "react";
import { Provider } from "react-redux";
import { store } from "../redux/store";

jest.mock("../Assets/images/banner_one.png");
jest.mock("../Assets/images/banner_two.png");
jest.mock("../Assets/images/banner_three.png");
window.scrollTo = jest.fn();

describe("App", () => {
  test("renders the app", () => {
    render(
      <Provider store={store}>
        <App />
      </Provider>
    );
    const app = screen.getByTestId("app");
    expect(app).toBeDefined();
  });
});
