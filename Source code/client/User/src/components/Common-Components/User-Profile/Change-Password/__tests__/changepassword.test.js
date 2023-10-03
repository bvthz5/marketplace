import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import ChangePassword from "../ChangePassword";
import { cartCounts } from "../../../../../App";
import userEvent from "@testing-library/user-event";
import { rest, server } from "../../../../../testServer";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

window.scrollTo = jest.fn();

const initialCartCount = 0;
const ChangePasswordWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <ChangePassword />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

test("should render changepassword component", async () => {
  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  const currentPasswordEyeButtonOpen = await screen.findByTestId(
    "eyebtn-currentpassword-open"
  );
  expect(currentPasswordEyeButtonOpen).toBeInTheDocument();

  const newPasswordEyeButtonOpen = await screen.findByTestId(
    "eyebtn-newpassword-open"
  );
  expect(newPasswordEyeButtonOpen).toBeInTheDocument();

  const confirmPswdEyeButtonOpen = await screen.findByTestId(
    "eyebtn-confirmpassword-open"
  );
  expect(confirmPswdEyeButtonOpen).toBeInTheDocument();

  fireEvent.click(currentPasswordEyeButtonOpen);
  fireEvent.click(newPasswordEyeButtonOpen);
  fireEvent.click(confirmPswdEyeButtonOpen);
  // ==============================================

  const currentPasswordEyeButtonClose = await screen.findByTestId(
    "eyebtn-currentpassword-close"
  );
  expect(currentPasswordEyeButtonClose).toBeInTheDocument();

  const newPasswordEyeButtonClose = await screen.findByTestId(
    "eyebtn-newpassword-close"
  );
  expect(newPasswordEyeButtonClose).toBeInTheDocument();

  const confirmPswdEyeButtonClose = await screen.findByTestId(
    "eyebtn-confirmpassword-close"
  );
  expect(confirmPswdEyeButtonClose).toBeInTheDocument();

  fireEvent.click(currentPasswordEyeButtonClose);
  fireEvent.click(newPasswordEyeButtonClose);
  fireEvent.click(confirmPswdEyeButtonClose);
});

test("add values to input fields current and get error messages", async () => {
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin12");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin12");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin1234");

  const error = await screen.findByTestId("confirm-error");
  expect(error).toHaveTextContent(
    /Password must contain 8 to 16 characters, at least one uppercase letter, one lowercase letter, one number and one special character/i
  );

  await user.type(screen.getByTestId(/new-password/i), "3");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin123");

  await user.type(screen.getByTestId(/new-password/i), "4");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin1234");

  await user.type(screen.getByTestId(/new-password/i), "5");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin12345");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add values to input fields new password and confirm password should't be same and then click submit", async () => {
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@12345");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@12345");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add values to input fields current and new password should't be same and then click submit", async () => {
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@123");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@123");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@123");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add valid data to input fields and then click submit and get success response", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/User/change-password",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json({ status: true }));
      }
    )
  );
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add valid data to input fields and then click submit and get error response (Password Not Set)", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/User/change-password",
      (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "Password Not Set" }));
      }
    )
  );
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add valid data to input fields and then click submit and get error response (Password MissMatch)", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/User/change-password",
      (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({ message: "Password MissMatch" })
        );
      }
    )
  );
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin");

  userEvent.clear(screen.getByTestId(/confirm-password/i));

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test("add valid data to input fields and then click submit and get error response (Unhandled error message)", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/User/change-password",
      (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({ message: "Unhandled error message" })
        );
      }
    )
  );
  const user = userEvent.setup();

  render(<ChangePasswordWrapper />);
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();

  await user.type(screen.getByTestId(/current-password/i), "Stejin@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId(/new-password/i), "Stejin@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Stejin@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Stejin@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Stejin@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
