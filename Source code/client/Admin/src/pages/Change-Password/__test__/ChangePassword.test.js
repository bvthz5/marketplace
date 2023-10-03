import "@testing-library/jest-dom/extend-expect";
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import ChangePassword from '../ChangePassword';
import userEvent from "@testing-library/user-event";
import {  rest,server } from "../../../testServer";



test('should render Change Password component', async() => {
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId('changepasswordpage');
  expect( changePasswordElement).toBeDefined();

  
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

  //////////////////////////////////////////////////


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
    render(
        <Router>
          <ChangePassword />
        </Router>
    );
    const changePasswordElement = screen.getByTestId("changepasswordpage");
    expect(changePasswordElement).toBeDefined();    

    await user.type(screen.getByTestId(/current-password/i), "Arun123");
    expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun123");
  
    await user.type(screen.getByTestId(/new-password/i), "Arun12");
    expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun12");
  
    await user.type(screen.getByTestId(/confirm-password/i), "Arun1234");
    expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun1234");
  
    await user.type(screen.getByTestId(/new-password/i), "3");
    expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun123");
  
    await user.type(screen.getByTestId(/new-password/i), "4");
    expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun1234");
  
    await user.type(screen.getByTestId(/new-password/i), "5");
    expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun12345");
  
    const submitButton = await screen.findByTestId("submit-button");
    expect(submitButton).toBeInTheDocument();
  
    fireEvent.click(submitButton);
});

test("add values to input fields new password and confirm password should't be same and then click submit", async () => {
  const user = userEvent.setup();
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();   

  await user.type(screen.getByTestId(/current-password/i), "Arun@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/new-password/i), "Arun@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Arun@12345");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun@12345");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
test("add values to input fields current and new password should't be same and then click submit", async () => {
  const user = userEvent.setup();
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();   


  await user.type(screen.getByTestId(/current-password/i), "Arun@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/new-password/i), "Arun@123");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/confirm-password/i), "Arun@123");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun@123");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
test("add valid data to input fields and then click submit and get success response", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/Admin/change-password",
      (req, res, ctx) => {
        return res(ctx.status(200), ctx.json({ status: true }));
      }
    )
  );
  const user = userEvent.setup();
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();   


  await user.type(screen.getByTestId(/current-password/i), "Arun@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/new-password/i), "Arun@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Arun@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
test("add valid data to input fields and then click submit and get error response (Password Not Set)", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/Admin/change-password",
      (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: "Password Not Set" }));
      }
    )
  );
  const user = userEvent.setup();
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();   

  await user.type(screen.getByTestId(/current-password/i), "Arun@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/new-password/i), "Arun@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Arun@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});


test("add valid data to input fields and then click submit and get error response (Password MissMatch)", async () => {
  server.use(
    rest.put(
      "https://localhost:8080/api/Admin/change-password",
      (req, res, ctx) => {
        return res(
          ctx.status(400),
          ctx.json({ message: "Password MissMatch" })
        );
      }
    )
  );
  const user = userEvent.setup();
  render(
      <Router>
        <ChangePassword />
      </Router>
  );
  const changePasswordElement = screen.getByTestId("changepasswordpage");
  expect(changePasswordElement).toBeDefined();   


  await user.type(screen.getByTestId(/current-password/i), "Arun@123");
  expect(screen.getByTestId(/current-password/i)).toHaveValue("Arun@123");

  await user.type(screen.getByTestId(/new-password/i), "Arun@1234");
  expect(screen.getByTestId(/new-password/i)).toHaveValue("Arun@1234");

  await user.type(screen.getByTestId(/confirm-password/i), "Arun@1234");
  expect(screen.getByTestId(/confirm-password/i)).toHaveValue("Arun@1234");

  const submitButton = await screen.findByTestId("submit-button");
  expect(submitButton).toBeInTheDocument();

  fireEvent.click(submitButton);
});
