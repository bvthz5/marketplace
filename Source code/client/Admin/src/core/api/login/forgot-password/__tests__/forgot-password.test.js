import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import ForgotPassword from '../forgot-password';
import userEvent from '@testing-library/user-event';
import { rest, server } from '../../../../../testServer';

test('should render forgotpassword component enter invalid email and close the modal', async () => {
  const user = userEvent.setup();
  render(
    <Router>
      <ForgotPassword />
    </Router>
  );
  const forgotPasswordElement = screen.getByTestId('forgotpasswordpage');
  expect(forgotPasswordElement).toBeDefined();

  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await user.type(screen.getByTestId(/email-input/i), 'invmarketplace');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('invmarketplace');

  const closeButton = await screen.findByTestId('close-button');
  expect(closeButton).toBeInTheDocument();

  fireEvent.click(closeButton);
});

test('should render forgotpassword component enter valid email and click submit and get success response', async () => {
  const user = userEvent.setup();
  server.use(
    rest.put('https://localhost:8080/api/Admin/forgot-password', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json({ status: true }));
    })
  );

  render(
    <Router>
      <ForgotPassword />
    </Router>
  );
  const forgotPasswordElement = screen.getByTestId('forgotpasswordpage');
  expect(forgotPasswordElement).toBeDefined();

  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await user.type(screen.getByTestId(/email-input/i), 'invmarketplace4u@gmail.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('invmarketplace4u@gmail.com');

  const submitButton = await screen.findByTestId('submit-button');
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});

test('should render forgotpassword component enter valid email and click submit and get error response(User Not Found)', async () => {
  const user = userEvent.setup();

  server.use(
    rest.put('https://localhost:8080/api/Admin/forgot-password', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'User Not Found' }));
    })
  );

  render(
    <Router>
      <ForgotPassword />
    </Router>
  );
  const forgotPasswordElement = screen.getByTestId('forgotpasswordpage');
  expect(forgotPasswordElement).toBeDefined();

  const forgotPasswordButton = await screen.findByTestId('forgotpasswordbutton');
  expect(forgotPasswordButton).toBeInTheDocument();
  fireEvent.click(forgotPasswordButton);

  const passwordmodal = await screen.findByTestId('passwordmodal');
  expect(passwordmodal).toBeInTheDocument();

  await user.type(screen.getByTestId(/email-input/i), 'invmarketplace4u@gmail.com');
  expect(screen.getByTestId(/email-input/i)).toHaveValue('invmarketplace4u@gmail.com');

  const submitButton = await screen.findByTestId('submit-button');
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
