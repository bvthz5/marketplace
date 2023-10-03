import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import userEvent from '@testing-library/user-event';
import ForgotPasswordAgent from '../forgotpassword-agent';
import { rest, server } from '../../../testServer';

test('should render forgotPasswordAgent component', async () => {
  const agent = userEvent.setup();
  render(
    <Router>
      <ForgotPasswordAgent />
    </Router>
  );
  const ForgotPasswordAgentElement = screen.getByTestId('forgotpasswordagentpage');
  expect(ForgotPasswordAgentElement).toBeDefined();


  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await agent.type(screen.getByTestId(/email-input/i), 'agent');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('agent');

  const closeButton = await screen.findByTestId('close-button');
  expect(closeButton).toBeInTheDocument();

  fireEvent.click(closeButton);
});

test('should render forgotpassword component enter valid email and click submit and get success response', async () => {
  const user = userEvent.setup();
  server.use(
    rest.put('https://localhost:8080/api/agent/forgot-password', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json({ status: true }));
    })
  );

  render(
    <Router>
      <ForgotPasswordAgent />
    </Router>
  );
  const forgotPasswordElement = screen.getByTestId('forgotpasswordagentpage');
  expect(forgotPasswordElement).toBeDefined();

  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await user.type(screen.getByTestId(/email-input/i), 'arun.george@innovaturelabs.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('arun.george@innovaturelabs.com');

  const submitButton = await screen.findByTestId('submit-button');
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});


test('should render forgotpassword component enter valid email and click submit and get error response(Agent not found)', async () => {
  const user = userEvent.setup();

  server.use(
    rest.put('https://localhost:8080/api/agent/forgot-password', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Agent not found' }));
    })
  );

  render(
    <Router>
    <ForgotPasswordAgent />
    </Router>
  );
  const forgotPasswordElement = screen.getByTestId('forgotpasswordagentpage');
  expect(forgotPasswordElement).toBeDefined();

  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await user.type(screen.getByTestId(/email-input/i), 'arun.george@innovaturelabs.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('arun.george@innovaturelabs.com');

  const submitButton = await screen.findByTestId('submit-button');
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
