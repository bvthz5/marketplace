
import React from "react";
import { render } from "@testing-library/react";
import ProtectedUsersRoutes from "../ProtectedUsersRoutes";

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  Navigate: jest.fn(),
}));
test("render userroutes component with accessToken", async () => {
  localStorage.setItem("accessToken", "testtoken");
  render(<ProtectedUsersRoutes />);
});

test("render loginprotection component without accessToken", async () => {
  localStorage.clear();
  render(<ProtectedUsersRoutes />);
});
