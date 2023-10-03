import {fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import userEvent from "@testing-library/user-event";
import VerifyToken from '../verify_token';
import { rest, server } from "../../../../../testServer";

jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useSearchParams: () => [new URLSearchParams({ token: "testingtoken" })],
}));

describe( "verify password component", () => {
test('should render verifytoken component', async() => {
  render(
      <Router>
        <VerifyToken />
      </Router>
  );
  const verifyTokenElement = screen.getByTestId('verifytokenpage');
  expect(verifyTokenElement).toBeDefined();

  const newPasswordEyeButtonOpen = await screen.findByTestId(
    "eyebtn-newpassword-open"
  );
  expect(newPasswordEyeButtonOpen).toBeInTheDocument();

  const confirmPswdEyeButtonOpen = await screen.findByTestId(
    "eyebtn-confirmpassword-open"
  );
  expect(confirmPswdEyeButtonOpen).toBeInTheDocument();

  fireEvent.click(newPasswordEyeButtonOpen);
  fireEvent.click(confirmPswdEyeButtonOpen);
  // ==============================================

  const newPasswordEyeButtonClose = await screen.findByTestId(
    "eyebtn-newpassword-close"
  );
  expect(newPasswordEyeButtonClose).toBeInTheDocument();

  const confirmPswdEyeButtonClose = await screen.findByTestId(
    "eyebtn-confirmpassword-close"
  );
  expect(confirmPswdEyeButtonClose).toBeInTheDocument();

  fireEvent.click(newPasswordEyeButtonClose);
  fireEvent.click(confirmPswdEyeButtonClose);


});
test("enter invalid values to input fields", async () => {
  const user = userEvent.setup();

  render(
    <Router>
      <VerifyToken />
    </Router>
  );
  const verifyTokenElement = screen.getByTestId("verifytokenpage");
  expect(verifyTokenElement).toBeDefined();

  await user.type(screen.getByTestId(/new-password/i), "Admin12");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin12");

  await user.type(screen.getByTestId(/confirm-password/i), "Admin1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Admin1234");

  await user.type(screen.getByTestId(/new-password/i), "3");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin123");

  await user.type(screen.getByTestId(/new-password/i), "4");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin1234");

  await user.type(screen.getByTestId(/new-password/i), "5");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin12345");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
test("add values to input fields new password and confirm password should't be same and then click submit", async () => {
  const user = userEvent.setup();

  render(
    <Router>
      <VerifyToken />
    </Router>
  );
  const verifyTokenElement = screen.getByTestId("verifytokenpage");
  expect(verifyTokenElement).toBeDefined();

  await user.type(screen.getByTestId(/new-password/i), "Admin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Admin@12345");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Admin@12345");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add valid data to input fields and then click submit and get success response", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/Admin/reset-password",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json({ status: true }));
      }
    )
  );
  const user = userEvent.setup();

  render(
    <Router>
      <VerifyToken />
    </Router>
  );
  const verifyTokenElement = screen.getByTestId("verifytokenpage");
  expect(verifyTokenElement).toBeDefined();

  await user.type(screen.getByTestId(/new-password/i), "Admin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Admin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Admin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});


test("add valid data to input fields and then click submit and get error response", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/Admin/reset-password",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );
  const user = userEvent.setup();

  render(
    <Router>
      <VerifyToken />
    </Router>
  );
  const verifyTokenElement = screen.getByTestId("verifytokenpage");
  expect(verifyTokenElement).toBeDefined();

  await user.type(screen.getByTestId(/new-password/i), "Admin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Admin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Admin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Admin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
});