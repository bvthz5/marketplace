
import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import NotificationBox from "../NotificationBox";
import { server, rest } from "../../../../../testServer";
import { countData } from "../testData/data";
import { notificationListData } from "../Notifications/testData/data";

describe("notificationbox", () => {
  test("renders notification component and fail to call list api while user is not loggedin", async () => {
    localStorage.clear();
    const callNoLogin = jest.fn();

    render(<NotificationBox loggedIn={false} callNoLogin={callNoLogin} />);
    expect(screen.getByTestId("notificationboxbtn")).toBeInTheDocument();
  });

  test("renders notification component", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/notification/count",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(countData));
        }
      )
    );
    server.use(
      rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(notificationListData));
      })
    );

    localStorage.setItem("accessToken", "temptoken");
    const callNoLogin = jest.fn();

    const handleCount = jest.fn();

    render(
      <NotificationBox
        loggedIn={true}
        callNoLogin={callNoLogin}
        callNotification={true}
        handleCount={handleCount}
      />
    );
    const notificationButton = screen.getByTestId("notificationboxbtn");

    expect(notificationButton).toBeInTheDocument();

    fireEvent.click(notificationButton);
  });

  test("renders notification component and get error response on list api", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/notification/count",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );
    localStorage.setItem("accessToken", "temptoken");
    const callNoLogin = jest.fn();

    render(
      <NotificationBox callNoLogin={callNoLogin} callNotification={true} />
    );
    const notificationButton = screen.getByTestId("notificationboxbtn");
    expect(notificationButton).toBeInTheDocument();
  });
});
