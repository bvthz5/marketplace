import React from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { server, rest } from "../../../../../../../testServer";
import CancelOrder from "../CancelOrder";
import userEvent from "@testing-library/user-event";

jest.mock("react-router-dom", () => ({
  useNavigate: jest.fn(),
}));

describe("cancel order", () => {
  test("should render cancel order button then open modal and then close it", async () => {
    const orderDetails = { orderDetailsId: 10, orderStatus: 5 };

    render(<CancelOrder orderDetails={orderDetails} />);

    const cancelButton = await screen.findByTestId("cancelorderbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);

    const modalHeading = await screen.findByText(/Reason for Cancelling/i);
    expect(modalHeading).toBeInTheDocument();

    const canceButton = await screen.findByTestId("modalclosebtn");
    expect(canceButton).toBeInTheDocument();

    fireEvent.click(canceButton);
  });

  test("should render cancel order button component get error messages on input field for not entering reason", async () => {
    server.use(
      rest.post(
        "https://localhost:8080/api/Order/refund/10",
        (req, res, ctx) => {
          return res(
            ctx.status(200),
            ctx.json({
              data: null,
              message: "Refund processed successfully",
              serviceStatus: 200,
              status: true,
            })
          );
        }
      )
    );

    const user = userEvent.setup();

    const orderDetails = { orderDetailsId: 10, orderStatus: 5 };

    render(<CancelOrder orderDetails={orderDetails} />);

    const cancelButton = await screen.findByTestId("cancelorderbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);

    const modalHeading = await screen.findByText(/Reason for Cancelling/i);
    expect(modalHeading).toBeInTheDocument();

    const submitButton = await screen.findByTestId("submit-btn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);

    await user.type(screen.getByTestId(/cancel-reason/i), "  ");
    expect(screen.getByTestId(/cancel-reason/i)).toHaveValue("  ");

    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("should render cancel order button component get success response on order cancellation", async () => {
    server.use(
      rest.post(
        "https://localhost:8080/api/Order/refund/10",
        (req, res, ctx) => {
          return res(
            ctx.status(200),
            ctx.json({
              data: null,
              message: "Refund processed successfully",
              serviceStatus: 200,
              status: true,
            })
          );
        }
      )
    );

    const user = userEvent.setup();

    const orderDetails = { orderDetailsId: 10, orderStatus: 5 };

    render(<CancelOrder orderDetails={orderDetails} />);

    const cancelButton = await screen.findByTestId("cancelorderbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);

    const modalHeading = await screen.findByText(/Reason for Cancelling/i);
    expect(modalHeading).toBeInTheDocument();

    await user.type(screen.getByTestId(/cancel-reason/i), "nothing");
    expect(screen.getByTestId(/cancel-reason/i)).toHaveValue("nothing");

    const submitButton = await screen.findByTestId("submit-btn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });

  test("should render cancel order button component and the button is disabled", async () => {
    const orderDetails = { orderDetailsId: 10, orderStatus: 6 };

    render(<CancelOrder orderDetails={orderDetails} />);

    const cancelButton = await screen.findByTestId("cancelorderbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);
  });

  test("should render cancel order button component get error response on order cancellation", async () => {
    server.use(
      rest.post(
        "https://localhost:8080/api/Order/refund/10/20",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );

    const user = userEvent.setup();

    render(<CancelOrder orderId={10} productId={20} />);

    const cancelButton = await screen.findByTestId("cancelorderbtn");
    expect(cancelButton).toBeInTheDocument();

    fireEvent.click(cancelButton);

    const modalHeading = await screen.findByText(/Reason for Cancelling/i);
    expect(modalHeading).toBeInTheDocument();

    await user.type(screen.getByTestId(/cancel-reason/i), "nothing");
    expect(screen.getByTestId(/cancel-reason/i)).toHaveValue("nothing");

    const submitButton = await screen.findByTestId("submit-btn");
    expect(submitButton).toBeInTheDocument();

    fireEvent.click(submitButton);
  });
});
