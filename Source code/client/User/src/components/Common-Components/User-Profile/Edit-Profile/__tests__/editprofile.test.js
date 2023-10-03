import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import EditProfile from "../EditProfile";
import { rest, server } from "../../../../../testServer";
import {
  userProfileData,
  userProfileDataWithImage,
} from "../../User-Profile/testData/data";
import userEvent from "@testing-library/user-event";
import { act } from "react-dom/test-utils";
import { Provider } from "react-redux";
import { store } from "../../../../../redux/store";

server.use(
  rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(userProfileData));
  })
);

const setData = async () => {
  const user = userEvent.setup();
  await act(async () => {
    await user.type(screen.getByTestId("fname-input"), " s");
    expect(screen.getByTestId("fname-input")).toHaveValue("Stejins");

    await user.type(screen.getByTestId("lname-input"), " j");
    expect(screen.getByTestId("lname-input")).toHaveValue("Jacob j");

    const submitButton = await screen.findByTestId("submitbtn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });
};

describe("edit user details", () => {
  test("should render editprofile component and get success response and click update button", async () => {
    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();

    const updateButton = await screen.findByTestId("updatebtn");
    expect(updateButton).toBeInTheDocument();

    fireEvent.click(updateButton);

    const cancelButton = await screen.findByTestId("cancelbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);
  });

  test("add invalid value to the input fields", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );

    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();

    const updateButton = await screen.findByTestId("updatebtn");
    expect(updateButton).toBeInTheDocument();

    fireEvent.click(updateButton);

    await act(async () => {
      await user.type(
        screen.getByTestId("fname-input"),
        " uiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
      );
      expect(screen.getByTestId("fname-input")).toHaveValue(
        "Stejinuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
      );

      await user.type(
        screen.getByTestId("lname-input"),
        "yuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
      );
      expect(screen.getByTestId("lname-input")).toHaveValue(
        "Jacobyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuio"
      );

      await user.type(screen.getByTestId("address-input"), "~");
      expect(screen.getByTestId("address-input")).toHaveValue("~");

      await user.type(screen.getByTestId("city-input"), "~");
      expect(screen.getByTestId("city-input")).toHaveValue("~");

      await user.type(screen.getByTestId("district-input"), "~");
      expect(screen.getByTestId("district-input")).toHaveValue("~");

      await user.type(screen.getByTestId("state-input"), "~");
      expect(screen.getByTestId("state-input")).toHaveValue("~");

      await user.type(screen.getByTestId("phone-input"), "123");
      expect(screen.getByTestId("phone-input")).toHaveValue("123");

      const submitButton = await screen.findByTestId("submitbtn");
      expect(submitButton).toBeInTheDocument();

      fireEvent.click(submitButton);
    });
  });

  test("submit the form with whitespace on first name field and error message", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileDataWithImage));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );

    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();

    const updateButton = await screen.findByTestId("updatebtn");
    expect(updateButton).toBeInTheDocument();

    fireEvent.click(updateButton);
    await act(async () => {
      
      await user.type(screen.getByTestId("fname-input"), " d");
      userEvent.clear(await screen.findByTestId("fname-input"));
      expect(screen.getByTestId("fname-input")).toHaveValue("");

      const submitButton = await screen.findByTestId("submitbtn");
      expect(submitButton).toBeInTheDocument();

      fireEvent.click(submitButton);
    });
  });

  test("should render editprofile component and get success response submit the form", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );
    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();

    const updateButton = await screen.findByTestId("updatebtn");
    expect(updateButton).toBeInTheDocument();

    fireEvent.click(updateButton);

    await setData();
  });

  test("submit the form and get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(userProfileData));
      })
    );

    server.use(
      rest.put("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();

    const updateButton = await screen.findByTestId("updatebtn");
    expect(updateButton).toBeInTheDocument();

    fireEvent.click(updateButton);

    await setData();
  });

  test("should render editprofile component and get error response", () => {
    server.use(
      rest.get("https://localhost:8080/api/User", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(
      <Provider store={store}>
        <Router>
          <EditProfile />
        </Router>
      </Provider>
    );
    const editProfileElement = screen.getByTestId("editprofile");
    expect(editProfileElement).toBeDefined();
  });
});
