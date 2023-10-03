import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { server, rest } from "../../../../../../testServer";
import StepperView from "../StepperView";
import { deliveredOrderHistory, orderHistory } from "../testData/data";

window.scrollTo = jest.fn();

describe("stepper", () => {
  test("should render stepperview component get success response on history api", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/order/history/5",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(orderHistory));
        }
      )
    );

    const orderDetails = { orderDetailsId: 5 };

    render(
      <StepperView
        orderDetails={orderDetails}
        orderStatus={6}
        horizontal={true}
      />
    );
    const stepButton = await screen.findAllByTestId("CheckCircleIcon");
    expect(stepButton[0]).toBeInTheDocument();
    fireEvent.click(stepButton[0]);
    fireEvent.click(stepButton[1]);
    fireEvent.click(stepButton[0]);
    fireEvent.click(stepButton[0]);
  });

  test("render stepperview component  with horizontal true", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/order/history/5",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(deliveredOrderHistory));
        }
      )
    );
    const orderDetails = { orderDetailsId: 5 };

    render(
      <StepperView
        orderDetails={orderDetails}
        orderStatus={7}
        horizontal={true}
      />
    );
    const stepButton = await screen.findAllByTestId("CheckCircleIcon");
    expect(stepButton[0]).toBeInTheDocument();
    fireEvent.click(stepButton[0]);
  });

  test("should stepperview  component with horizontal false", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/order/history/5",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );
    const orderDetails = { orderDetailsId: 5 };

    render(
      <StepperView
        orderDetails={orderDetails}
        orderStatus={5}
        horizontal={false}
      />
    );
  });
});
