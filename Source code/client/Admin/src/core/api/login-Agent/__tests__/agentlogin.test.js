import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import { GoogleOAuthProvider } from '@react-oauth/google';
import userEvent from '@testing-library/user-event';
import AgentLogin from '../login-agent-page/AgentLogin';
import { rest, server } from '../../../../testServer';
import { agentLoginData, agentLoginDataInactive } from '../testData/data';
import { Provider } from 'react-redux';
import { store } from '../../../../redux/store';


const CLIENT_ID = process.env.REACT_APP_GOOGLE_CLIENT_ID;

const setData = async () => {
  const agent = userEvent.setup();
  await agent.type(screen.getByTestId(/email-input/i), 'arun.george@innovaturelabs.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('arun.george@innovaturelabs.com');
  await agent.type(screen.getByTestId(/password-input/i), 'Arun@123');
  expect(screen.getByTestId(/password-input/i)).toHaveValue('Arun@123');
};
test('should render login component', async () => {
  render(

 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  
    
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
  
  const goback = await screen.findByTestId('goback');
  expect(goback).toBeInTheDocument();
  fireEvent.click(goback);
});
test('clicking on go back icon', async () => {
  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  const gobackicon = await screen.findByTestId('goback');
  expect(gobackicon).toBeInTheDocument();

  fireEvent.click(gobackicon);

});
test('enter invalid values to input fields', async () => {
  const agent = userEvent.setup();

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await agent.type(screen.getByTestId(/email-input/i), 'Arun');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('Arun');

  await agent.type(screen.getByTestId(/password-input/i), 'Arun123');
  expect(screen.getByTestId(/password-input/i)).toHaveValue('Arun123');

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});

test('enter valid email and password then click submit and get success response', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(agentLoginData));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
test('enter valid email and password then click submit and get success response in inactive agent', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(agentLoginDataInactive));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
test('enter valid email and password then click submit and get error response(Invalid Credentials)', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Invalid Credentials' }));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});

test('enter valid email and password then click submit and get erro response(Agent BLOCKED)', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Agent BLOCKED' }));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});

test('enter valid email and password then click submit and get error response(Password not set)', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Password not set' }));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
test('enter valid email and password then click submit and get error response(Agent not found)', async () => {
  server.use(
    rest.post('https://localhost:8080/api/login/agent', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Agent not found' }));
    })
  );

  render(
 <Provider store={store}>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <Router>
        <AgentLogin />
      </Router>
    </GoogleOAuthProvider>
    </Provider>
  );
  const loginElement = screen.getByTestId('loginpage');
  expect(loginElement).toBeDefined();

  await setData();

  const loginButton = await screen.findByTestId('login-button');
  expect(loginButton).toBeInTheDocument();

  fireEvent.click(loginButton);
});
