import {  render, screen, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import VerifyEmail from "../Verify-email";
import { rest, server } from "../../../testServer";
import Swal from "sweetalert2";

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ token: "testtoken" })],
}));


server.use(
  rest.put("https://localhost:8080/api/User/verify", (req, res, ctx) => {
    return res(
      ctx.status(200),
      ctx.json({
        data: null,
        message: "User verified",
        serviceStatus: 200,
        status: true,
      })
    );
  })
);

describe("veryfy component", () => {
  test("should get success response on verifying token", () => {
    jest.mock("sweetalert2", () => ({
      fire: jest.fn().mockResolvedValue({ isConfirmed: true }),
    }));

    render(
      <Router>
        <VerifyEmail />
      </Router>
    );
    const verifyEmailElement = screen.getByTestId("verifyemailpage");
    expect(verifyEmailElement).toBeDefined();
  });

  test("should get error response on verifying token", async () => {
    server.use(
      rest.put("https://localhost:8080/api/User/verify", (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({
            data: null,
            message: "Token Expired",
            serviceStatus: 400,
            status: true,
          })
        );
      })
    );

    const SwalFireSpy = jest.spyOn(Swal, "fire");
    render(
      <Router>
        <VerifyEmail />
      </Router>
    );
    const verifyEmailElement = screen.getByTestId("verifyemailpage");
    expect(verifyEmailElement).toBeDefined();

    await waitFor(() => expect(SwalFireSpy).toHaveBeenCalled());
  });
});
