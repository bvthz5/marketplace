
import React from "react";
import { render } from "@testing-library/react";
import LoginProtection from "../LoginProtection";

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  Navigate: jest.fn(),
}));
describe("login protection", () => {
  test("render loginprotection component with accessToken", async () => {
    localStorage.setItem("accessToken", "testtoken");
    render(<LoginProtection />);
  });
  test("render loginprotection component without accessToken", async () => {
    localStorage.clear();
    render(<LoginProtection />);
  });
});
