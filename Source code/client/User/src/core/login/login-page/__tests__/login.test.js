import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import Login from "../login";
import { GoogleOAuthProvider } from "@react-oauth/google";
import userEvent from "@testing-library/user-event";
import { rest, server } from "../../../../testServer";
import { userLoginData } from "../testData/data";
import { store } from "../../../../redux/store";
import { Provider } from "react-redux";

jest.mock("react-top-loading-bar");

const CLIENT_ID = process.env.REACT_APP_GOOGLE_CLIENT_ID;

const setData = async () => {
  const user = userEvent.setup();

  await user.type(
    screen.getByTestId(/email-input/i),
    "stejin.jacob@innovaturelabs.com"
  );
  expect(screen.getByTestId(/email-input/i)).toHaveValue(
    "stejin.jacob@innovaturelabs.com"
  );

  await user.type(screen.getByTestId(/password-input/i), "Stejin@123");
  expect(screen.getByTestId(/password-input/i)).toHaveValue("Stejin@123");
};

describe("login component", () => {
  test("should render login component", async () => {
    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    const passwordEyeButtonOpen = await screen.findByTestId(
      "eyebtn-password-open"
    );
    expect(passwordEyeButtonOpen).toBeInTheDocument();

    fireEvent.click(passwordEyeButtonOpen);

    const passwordEyeButtonClose = await screen.findByTestId(
      "eyebtn-password-close"
    );
    expect(passwordEyeButtonClose).toBeInTheDocument();
    fireEvent.click(passwordEyeButtonClose);

    const registerLink = await screen.findByTestId("register-link");
    expect(registerLink).toBeInTheDocument();
    fireEvent.click(registerLink);
  });

  test("enter invalid values to input fields", async () => {
    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    await user.type(screen.getByTestId(/email-input/i), "Stejin");
    expect(screen.getByTestId(/email-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/password-input/i), "Stejin123");
    expect(screen.getByTestId(/password-input/i)).toHaveValue("Stejin123");

    const loginButton = await screen.findByTestId("login-button");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  test("enter valid email and password then click submit and get success response", async () => {
    server.use(
      rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userLoginData));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    await setData();

    const loginButton = await screen.findByTestId("login-button");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  //===============================Invalid Credentials========================================

  test("enter valid email and password then click submit and get error response(Invalid Credentials)", async () => {
    server.use(
      rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({ message: "Invalid Credentials" })
        );
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    await setData();

    const loginButton = await screen.findByTestId("login-button");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  //===============================User Blocked========================================
  test("enter valid email and password then click submit and get erro response(User Blocked)", async () => {
    server.use(
      rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "User Blocked" }));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    await setData();

    const loginButton = await screen.findByTestId("login-button");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  //===============================User not verified========================================
  describe("enter valid email and password then click submit and get erro response(Us er not verified)", () => {
    //                 -----------------Resend email success-----------------------
    test("click resend button on the pop-up and get success response", async () => {
      server.use(
        rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
          return res(
            ctx.status(400),
            ctx.json({ message: "User not verified" })
          );
        })
      );

      server.use(
        rest.put(
          "https://localhost:8080/api/User/resend-verification-mail",
          (req, res, ctx) => {
            return res(ctx.status(200));
          }
        )
      );

      render(
        <Provider store={store}>
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <Router>
              <Login />
            </Router>
          </GoogleOAuthProvider>
        </Provider>
      );
      const loginElement = screen.getByTestId("loginpage");
      expect(loginElement).toBeDefined();

      await setData();

      const loginButton = await screen.findByTestId("login-button");
      expect(loginButton).toBeInTheDocument();
      fireEvent.click(loginButton);

      const resendButton = await screen.findByText(/Resend-Email/i);
      expect(resendButton).toBeInTheDocument();
      fireEvent.click(resendButton);
    });

    //                 -----------------Resend email failure-----------------------
    test("click resend button on the pop-up and get error response", async () => {
      server.use(
        rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
          return res(
            ctx.status(400),
            ctx.json({ message: "User not verified" })
          );
        })
      );

      server.use(
        rest.put(
          "https://localhost:8080/api/User/resend-verification-mail",
          (req, res, ctx) => {
            return res(ctx.status(400));
          }
        )
      );

      render(
        <Provider store={store}>
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <Router>
              <Login />
            </Router>
          </GoogleOAuthProvider>
        </Provider>
      );
      const loginElement = screen.getByTestId("loginpage");
      expect(loginElement).toBeDefined();

      await setData();

      const loginButton = await screen.findByTestId("login-button");
      expect(loginButton).toBeInTheDocument();
      fireEvent.click(loginButton);

      const resendButton = await screen.findByText(/Resend-Email/i);
      expect(resendButton).toBeInTheDocument();
      fireEvent.click(resendButton);
    });

    //                 -----------------close resend email pop-up-----------------------
    test("click cancel button on the pop-up", async () => {
      server.use(
        rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
          return res(
            ctx.status(400),
            ctx.json({ message: "User not verified" })
          );
        })
      );

      render(
        <Provider store={store}>
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <Router>
              <Login />
            </Router>
          </GoogleOAuthProvider>
        </Provider>
      );
      const loginElement = screen.getByTestId("loginpage");
      expect(loginElement).toBeDefined();

      await setData();

      const loginButton = await screen.findByTestId("login-button");
      expect(loginButton).toBeInTheDocument();
      fireEvent.click(loginButton);

      const cancelButton = await screen.findByText(/Cancel/i);
      expect(cancelButton).toBeInTheDocument();
      fireEvent.click(cancelButton);
    });
  });

  //===============================Password not set========================================
  test("enter valid email and password then click submit and get error response(Password not set)", async () => {
    server.use(
      rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "Password not set" }));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Login />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const loginElement = screen.getByTestId("loginpage");
    expect(loginElement).toBeDefined();

    await setData();

    const loginButton = await screen.findByTestId("login-button");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });
});
