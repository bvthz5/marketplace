
import React from "react";
import {
  fireEvent,
  render,
  screen,
} from "@testing-library/react";
import Notifications from "../Notifications";
import { notificationListData, notificationListType2Data } from "../testData/data";
import { server, rest } from "../../../../../../testServer";

jest.mock(
  "../../../../../Assets/images/orange-notification-bell-icon-png-11638985058ine0rbglzz.png"
);
jest.mock("../../../../../Assets/images/icon-notification.png");

test("renders notification component and fail to call list api", async () => {
  localStorage.clear();
  const handleCount = jest.fn();

  render(<Notifications handleCount={handleCount} />);
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();
});

test("renders notification component and get success response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(notificationListData));
    })
  );

  server.use(
    rest.put("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(<Notifications handleCount={handleCount} />);
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  const data = await screen.findByText(/Apple Watch Ultra/i);
  expect(data).toBeInTheDocument();
});

test("renders notification component and delete notifications by id and get success response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(notificationListType2Data));
    })
  );

  server.use(
    rest.put("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  server.use(
    rest.delete(
      "https://localhost:8080/api/notification/95",
      (req, res, ctx) => {
        return res(ctx.status(200));
      }
    )
  );

  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(
    <Notifications handleCount={handleCount} />
  );
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  const data = await screen.findByText(/Apple Watch Ultra/i);
  expect(data).toBeInTheDocument();

  const deletebutton = await screen.findByTestId("deletebutton");
  fireEvent.click(deletebutton);
});

test("renders notification component and delete notifications by id and get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(notificationListData));
    })
  );

  server.use(
    rest.put("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  server.use(
    rest.delete(
      "https://localhost:8080/api/notification/95",
      (req, res, ctx) => {
        return res(ctx.status(400));
      }
    )
  );

  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(
    <Notifications handleCount={handleCount} />
  );
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  const data = await screen.findByText(/Apple Watch Ultra/i);
  expect(data).toBeInTheDocument();

  const deletebutton = await screen.findByTestId("deletebutton");
  fireEvent.click(deletebutton);
});

test("renders notification component and delete notifications by clear all button and get success response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(notificationListData));
    })
  );

  server.use(
    rest.put("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  server.use(
    rest.delete("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );

  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(
    <Notifications handleCount={handleCount} />
  );
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  const data = await screen.findByText(/Apple Watch Ultra/i);
  expect(data).toBeInTheDocument();

  const clearButton = await screen.findByTestId("clearallbtn");
  fireEvent.click(clearButton);
});

test("renders notification component and delete notifications by clear all button and get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(notificationListData));
    })
  );

  server.use(
    rest.put("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(200));
    })
  );
  server.use(
    rest.delete("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );

  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(
    <Notifications handleCount={handleCount} />
  );
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  const data = await screen.findByText(/Apple Watch Ultra/i);
  expect(data).toBeInTheDocument();

  const clearButton = await screen.findByTestId("clearallbtn");
  fireEvent.click(clearButton);
});

test("renders notification component and get error response", async () => {
  server.use(
    rest.get("https://localhost:8080/api/notification", (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  const handleCount = jest.fn();

  localStorage.setItem("accessToken", "temptoken");
render(<Notifications handleCount={handleCount} />);
  expect(screen.getByTestId("notificationlist")).toBeInTheDocument();

  // const data = await screen.findByText(/No notifications/i);
  // expect(data).toBeInTheDocument();
});
