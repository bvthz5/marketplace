import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import Login from '../login';
import { GoogleOAuthProvider } from '@react-oauth/google';
import userEvent from '@testing-library/user-event';
import { rest,server } from '../../../../../testServer';
import { adminLoginData } from '../testData/data';

const CLIENT_ID = process.env.REACT_APP_GOOGLE_CLIENT_ID;

const setData = async () => {
  const user = userEvent.setup();
  await user.type(screen.getByTestId(/email-input/i), 'invmarketlace4u@gmail.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('invmarketlace4u@gmail.com');
  await user.type(screen.getByTestId(/password-input/i), 'Admin@123');
  expect(screen.getByTestId(/password-input/i)).toHaveValue('Admin@123');
};
test('should render login component', async () => {
  render(
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <Login />
      </Router>
    </GoogleOAuthProvider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  const passwordEyeButtonOpen = await screen.findByTestId('eyebtn-password-open');
  expect(passwordEyeButtonOpen).toBeInTheDocument();

  fireEvent.click(passwordEyeButtonOpen);

  const passwordEyeButtonClose = await screen.findByTestId('eyebtn-password-close');
  expect(passwordEyeButtonClose).toBeInTheDocument();
  fireEvent.click(passwordEyeButtonClose);
  const forgotPasswordModal = await screen.findByTestId('forgot-password');
  expect(forgotPasswordModal).toBeInTheDocument();
  fireEvent.click(forgotPasswordModal);
  const agentclick = await screen.findByTestId('agentclick');
  expect(agentclick).toBeInTheDocument();

  fireEvent.click(agentclick);

});
test('enter invalid values to input fields', async () => {
  const user = userEvent.setup();
  render(
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <Login />
      </Router>
    </GoogleOAuthProvider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();
  await user.type(screen.getByTestId(/email-input/i), 'Admin');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('Admin');

  await user.type(screen.getByTestId(/password-input/i), 'Admin123');
  expect(screen.getByTestId(/password-input/i)).toHaveValue('Admin123');

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
test("enter valid email and password then click submit and get success response", async () => {
  server.use(
    rest.post("https://localhost:8080/api/login", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(adminLoginData));
    })
  );
  render(
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <Login />
      </Router>
    </GoogleOAuthProvider>
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
      return res(ctx.status(400), ctx.json({ message: "Invalid Credentials" }));
    })
  );
  render(
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <Login />
      </Router>
    </GoogleOAuthProvider>
  );
  const loginElement = screen.getByTestId("loginpage");
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId("login-button");
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});

//...................password not set.........................
test("enter valid email and password then click submit and get error response(Password not set)", async () => {
  server.use(
    rest.post("https://localhost:8080/api/Login", (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: "Password not set" }));
    })
  );

  render(
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <Login />
      </Router>
    </GoogleOAuthProvider>
  );
  const loginElement = screen.getByTestId("loginpage");
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId("login-button");
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
