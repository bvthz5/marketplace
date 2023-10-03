
import React from "react";
import { render } from "@testing-library/react";
import RazorPay from "../RazorPay";
import { BrowserRouter as Router } from "react-router-dom";
import { JSDOM } from "jsdom";

window.Razorpay = jest.fn(() => ({
    on: jest.fn(),
    open: jest.fn(),
  }));

const orderDetails = {
  orderId: 429,
  orderNumber: "order_LXdB4MzDrI2ZMI",
  updatedDate: "0001-01-01T00:00:00",
  email: "ananyarajee.v36@gmail.com",
  userId: 243,
  totalPrice: 20000,
  paymentStatus: 0,
};

test("render Razorpay component", async () => {
  const dom = new JSDOM();
  global.window = dom.window;
  global.document = window.document;
  render(
    <Router>
      <RazorPay orderDetails={orderDetails} />
    </Router>
  );
});
