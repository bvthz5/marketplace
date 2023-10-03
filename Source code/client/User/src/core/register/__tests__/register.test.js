import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import Register from "../register";
import { GoogleOAuthProvider } from "@react-oauth/google";
import userEvent from "@testing-library/user-event";
import { rest, server } from "../../../testServer";
import { Provider } from "react-redux";
import { store } from "../../../redux/store";

const CLIENT_ID = process.env.REACT_APP_GOOGLE_CLIENT_ID;

const setData = async () => {
  const user = userEvent.setup();

  await user.type(screen.getByTestId("fname-input"), "Stejin");
  expect(screen.getByTestId("fname-input")).toHaveValue("Stejin");

  await user.type(screen.getByTestId("email-input"), "stejin@gmail.com");
  expect(screen.getByTestId("email-input")).toHaveValue("stejin@gmail.com");

  await user.type(screen.getByTestId("password-input"), "Stejin@123");
  expect(screen.getByTestId("password-input")).toHaveValue("Stejin@123");

  await user.type(screen.getByTestId("confirm-password-input"), "Stejin@123");
  expect(screen.getByTestId("confirm-password-input")).toHaveValue(
    "Stejin@123"
  );
};

describe("register component", () => {
  test("should render register component", async () => {
    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Register />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const registerElement = screen.getByTestId("registerpage");
    expect(registerElement).toBeDefined();

    const backButton = await screen.findByTestId("back-button");
    expect(backButton).toBeInTheDocument();

    const passwordEyeButtonOpen = await screen.findByTestId(
      "eyebtn-password-open"
    );
    expect(passwordEyeButtonOpen).toBeInTheDocument();

    const confirmPswdEyeButtonOpen = await screen.findByTestId(
      "eyebtn-confirmpassword-open"
    );
    expect(confirmPswdEyeButtonOpen).toBeInTheDocument();

    fireEvent.click(backButton);
    fireEvent.click(passwordEyeButtonOpen);
    fireEvent.click(confirmPswdEyeButtonOpen);

    const passwordEyeButtonClose = await screen.findByTestId(
      "eyebtn-password-close"
    );
    expect(passwordEyeButtonClose).toBeInTheDocument();

    const confirmPswdEyeButtonClose = await screen.findByTestId(
      "eyebtn-confirmpassword-close"
    );
    expect(confirmPswdEyeButtonClose).toBeInTheDocument();

    fireEvent.click(passwordEyeButtonClose);
    fireEvent.click(confirmPswdEyeButtonClose);

    const loginButton = await screen.findByTestId("login-route");
    expect(loginButton).toBeInTheDocument();

    fireEvent.click(loginButton);
  });

  test("enter invalid values to input fields to cover error messages", async () => {
    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Register />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const registerElement = screen.getByTestId("registerpage");
    expect(registerElement).toBeDefined();

    await user.type(
      screen.getByTestId("fname-input"),
      "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
    );
    expect(screen.getByTestId("fname-input")).toHaveValue(
      "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
    );

    await user.type(
      screen.getByTestId("lname-input"),
      "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
    );
    expect(screen.getByTestId("lname-input")).toHaveValue(
      "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
    );

    await user.type(screen.getByTestId("email-input"), "Stejin");
    expect(screen.getByTestId("email-input")).toHaveValue("Stejin");

    await user.type(screen.getByTestId("password-input"), "Stejin12");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin12");

    await user.type(screen.getByTestId("confirm-password-input"), "Stejin1234");
    expect(screen.getByTestId("confirm-password-input")).toHaveValue(
      "Stejin1234"
    );

    await user.type(screen.getByTestId("password-input"), "3");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin123");

    await user.type(screen.getByTestId("password-input"), "4");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin1234");

    await user.type(screen.getByTestId("password-input"), "5");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin12345");

    const registerButton = await screen.findByTestId("register-button");
    expect(registerButton).toBeInTheDocument();

    fireEvent.click(registerButton);
  });

  test("enter valid values to input fields give diiffrent values in password and confirm password and submit", async () => {
    const user = userEvent.setup();
    server.use(
      rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Register />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const registerElement = screen.getByTestId("registerpage");
    expect(registerElement).toBeDefined();

    await user.type(screen.getByTestId("fname-input"), "  ");
    expect(screen.getByTestId("fname-input")).toHaveValue("  ");

    await user.type(screen.getByTestId("email-input"), "stejin@gmail.com");
    expect(screen.getByTestId("email-input")).toHaveValue("stejin@gmail.com");

    await user.type(screen.getByTestId("password-input"), "Stejin@123");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin@123");

    await user.type(
      screen.getByTestId("confirm-password-input"),
      "Stejin@1234"
    );
    expect(screen.getByTestId("confirm-password-input")).toHaveValue(
      "Stejin@1234"
    );

    const registerButton = await screen.findByTestId("register-button");
    expect(registerButton).toBeInTheDocument();

    fireEvent.click(registerButton);
  });

  test("enter valid values to input fields and submit to get whitespace error", async () => {
    const user = userEvent.setup();
    server.use(
      rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Register />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const registerElement = screen.getByTestId("registerpage");
    expect(registerElement).toBeDefined();

    await user.type(screen.getByTestId("fname-input"), "  ");
    expect(screen.getByTestId("fname-input")).toHaveValue("  ");

    await user.type(screen.getByTestId("email-input"), "stejin@gmail.com");
    expect(screen.getByTestId("email-input")).toHaveValue("stejin@gmail.com");

    await user.type(screen.getByTestId("password-input"), "Stejin@123");
    expect(screen.getByTestId("password-input")).toHaveValue("Stejin@123");

    await user.type(screen.getByTestId("confirm-password-input"), "Stejin@123");
    expect(screen.getByTestId("confirm-password-input")).toHaveValue(
      "Stejin@123"
    );

    const registerButton = await screen.findByTestId("register-button");
    expect(registerButton).toBeInTheDocument();

    fireEvent.click(registerButton);
  });

  test("enter valid values to input fields and submit to get success response", async () => {
    server.use(
      rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );

    render(
      <Provider store={store}>
        <GoogleOAuthProvider clientId={CLIENT_ID}>
          <Router>
            <Register />
          </Router>
        </GoogleOAuthProvider>
      </Provider>
    );
    const registerElement = screen.getByTestId("registerpage");
    expect(registerElement).toBeDefined();

    await setData();

    const registerButton = await screen.findByTestId("register-button");
    expect(registerButton).toBeInTheDocument();

    fireEvent.click(registerButton);
  });

  //===============================Error Responses========================================
  describe("error responses", () => {
    //===============================User Already Exists========================================
    describe("enter valid values to input fields and submit to get error response(User Already Exists)", () => {
      test("click login now button on the popup", async () => {
        server.use(
          rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
            return res(
              ctx.status(400),
              ctx.json({ message: "User Already Exists" })
            );
          })
        );

        render(
          <Provider store={store}>
            <GoogleOAuthProvider clientId={CLIENT_ID}>
              <Router>
                <Register />
              </Router>
            </GoogleOAuthProvider>
          </Provider>
        );
        const registerElement = screen.getByTestId("registerpage");
        expect(registerElement).toBeDefined();

        await setData();

        const registerButton = await screen.findByTestId("register-button");
        expect(registerButton).toBeInTheDocument();

        fireEvent.click(registerButton);

        const swalLoginBtn = await screen.findByText("Login Now");
        expect(swalLoginBtn).toBeInTheDocument();
        fireEvent.click(swalLoginBtn);
      });

      test("click cancel button on the popup", async () => {
        server.use(
          rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
            return res(
              ctx.status(400),
              ctx.json({ message: "User Already Exists" })
            );
          })
        );

        render(
          <Provider store={store}>
            <GoogleOAuthProvider clientId={CLIENT_ID}>
              <Router>
                <Register />
              </Router>
            </GoogleOAuthProvider>
          </Provider>
        );
        const registerElement = screen.getByTestId("registerpage");
        expect(registerElement).toBeDefined();

        await setData();

        const registerButton = await screen.findByTestId("register-button");
        expect(registerButton).toBeInTheDocument();

        fireEvent.click(registerButton);

        const swalCancelBtn = await screen.findByText("Cancel");
        expect(swalCancelBtn).toBeInTheDocument();
        fireEvent.click(swalCancelBtn);
      });
    });

    //===============================Inactive User========================================
    describe("enter valid values to input fields and submit to get error response(Inactive User)", () => {
      //                 -----------------Resend email success-----------------------
      test("click resend button on the pop-up and get success response", async () => {
        server.use(
          rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
            return res(ctx.status(400), ctx.json({ message: "Inactive User" }));
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
                <Register />
              </Router>
            </GoogleOAuthProvider>
          </Provider>
        );
        const registerElement = screen.getByTestId("registerpage");
        expect(registerElement).toBeDefined();

        await setData();

        const registerButton = await screen.findByTestId("register-button");
        expect(registerButton).toBeInTheDocument();

        fireEvent.click(registerButton);

        const resendButton = await screen.findByText(/Resend-Email/i);
        expect(resendButton).toBeInTheDocument();
        fireEvent.click(resendButton);
      });

      //                 -----------------Resend email failure-----------------------
      test("click resend button on the pop-up and get error response", async () => {
        server.use(
          rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
            return res(ctx.status(400), ctx.json({ message: "Inactive User" }));
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
                <Register />
              </Router>
            </GoogleOAuthProvider>
          </Provider>
        );
        const registerElement = screen.getByTestId("registerpage");
        expect(registerElement).toBeDefined();

        await setData();

        const registerButton = await screen.findByTestId("register-button");
        expect(registerButton).toBeInTheDocument();

        fireEvent.click(registerButton);

        const resendButton = await screen.findByText(/Resend-Email/i);
        expect(resendButton).toBeInTheDocument();
        fireEvent.click(resendButton);
      });

      //                 -----------------close resend email pop-up-----------------------
      test("click cancel button on the pop-up", async () => {
        server.use(
          rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
            return res(ctx.status(400), ctx.json({ message: "Inactive User" }));
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
                <Register />
              </Router>
            </GoogleOAuthProvider>
          </Provider>
        );
        const registerElement = screen.getByTestId("registerpage");
        expect(registerElement).toBeDefined();

        await setData();

        const registerButton = await screen.findByTestId("register-button");
        expect(registerButton).toBeInTheDocument();

        fireEvent.click(registerButton);

        const cancelButton = await screen.findByText(/Cancel/i);
        expect(cancelButton).toBeInTheDocument();
        fireEvent.click(cancelButton);
      });
    });

    //===============================Blocked User========================================
    test("enter valid values to input fields and submit to get error response(Blocked User)", async () => {
      server.use(
        rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
          return res(ctx.status(400), ctx.json({ message: "Blocked User" }));
        })
      );

      render(
        <Provider store={store}>
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <Router>
              <Register />
            </Router>
          </GoogleOAuthProvider>
        </Provider>
      );
      const registerElement = screen.getByTestId("registerpage");
      expect(registerElement).toBeDefined();

      await setData();

      const registerButton = await screen.findByTestId("register-button");
      expect(registerButton).toBeInTheDocument();

      fireEvent.click(registerButton);
    });

    //===============================Unhandled mesage========================================
    test("enter valid values to input fields and submit to get error response(Unhandled mesage)", async () => {
      server.use(
        rest.post("https://localhost:8080/api/User", (req, res, ctx) => {
          return res(
            ctx.status(400),
            ctx.json({ message: "Unhandled mesage" })
          );
        })
      );

      render(
        <Provider store={store}>
          <GoogleOAuthProvider clientId={CLIENT_ID}>
            <Router>
              <Register />
            </Router>
          </GoogleOAuthProvider>
        </Provider>
      );
      const registerElement = screen.getByTestId("registerpage");
      expect(registerElement).toBeDefined();

      await setData();

      const registerButton = await screen.findByTestId("register-button");
      expect(registerButton).toBeInTheDocument();

      fireEvent.click(registerButton);
    });
  });
});
