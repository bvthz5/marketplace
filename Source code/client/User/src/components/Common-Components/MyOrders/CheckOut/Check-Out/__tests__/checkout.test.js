import React, { useMemo, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import { cartCounts } from "../../../../../../App";
import Checkout from "../Checkout";
import { getCartData } from "../../../../MyCart/testData/data";
import {
  addressListData,
  createOrderResponse,
  multipleAddressListData,
} from "../testData/data";
import { rest, server } from "../../../../../../testServer";
import { addressEditData } from "../../Address-Form/testData/data";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

// ==================== test for checkout page while there is no product id ====================
// ==================== product id is not recieved when user clicks place order button from cart page ====================

window.scrollTo = jest.fn();

server.use(
  rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getCartData));
  })
);

server.use(
  rest.get("https://localhost:8080/api/delivery-address", (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(addressListData));
  })
);

const initialCartCount = 0;
const CheckoutWrapper = () => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <Checkout />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

describe("checkout page for cart products", () => {
  test("should render checkout component and go back due to no order in localstorage", async () => {
    localStorage.clear();
    render(<CheckoutWrapper />);
    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
  });
  test("should render checkout component without product id to get cart products", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressListData));
        }
      )
    );
    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);
    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Stejin Jacob/i);
    expect(name).toBeInTheDocument();
  });

  test("clicks on the diffrent buttons in the address list", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(multipleAddressListData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address/delete/2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address/2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressEditData));
        }
      )
    );

    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);
    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Arun George/i);
    expect(name).toBeInTheDocument();

    const checkBox = await screen.findByTestId("checkbox-2");
    expect(checkBox).toBeInTheDocument();

    fireEvent.click(checkBox);

    const addressDeleteBtn1 = await screen.findByTestId("addressdeletebtn-1");
    expect(addressDeleteBtn1).toBeInTheDocument();

    fireEvent.click(addressDeleteBtn1);

    const addressDeleteBtn2 = await screen.findByTestId("addressdeletebtn-2");
    expect(addressDeleteBtn2).toBeInTheDocument();

    fireEvent.click(addressDeleteBtn2);

    const swalDeleteBtn = await screen.findByText(/Delete/i);
    expect(swalDeleteBtn).toBeInTheDocument();

    fireEvent.click(swalDeleteBtn);

    const deliverHereBtn = await screen.findByTestId("deliverHere");
    expect(deliverHereBtn).toBeInTheDocument();

    fireEvent.click(deliverHereBtn);

    const changeButton = await screen.findByTestId("addresschangebtn");
    expect(changeButton).toBeInTheDocument();

    fireEvent.click(changeButton);

    const addressEditBtn = await screen.findByTestId("editAddress-2");
    expect(addressEditBtn).toBeInTheDocument();

    fireEvent.click(addressEditBtn);
  });

  test("delete address and get error response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(multipleAddressListData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address/delete/2",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );

    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);
    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Arun George/i);
    expect(name).toBeInTheDocument();

    const addressDeleteBtn2 = await screen.findByTestId("addressdeletebtn-2");
    expect(addressDeleteBtn2).toBeInTheDocument();

    fireEvent.click(addressDeleteBtn2);

    const swalDeleteBtn = await screen.findByText(/Delete/i);
    expect(swalDeleteBtn).toBeInTheDocument();

    fireEvent.click(swalDeleteBtn);
  });

  test("clicks on buy now button and then  cancels the  order and  get success responses", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressListData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.post("https://localhost:8080/api/Order", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(createOrderResponse));
      })
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/Order/cancel?orderNumber=testOrderNumber",
        (req, res, ctx) => {
          return res(ctx.status(200));
        }
      )
    );

    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);

    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Stejin Jacob/i);
    expect(name).toBeInTheDocument();

    const deliverHereBtn = await screen.findByTestId("deliverHere");
    expect(deliverHereBtn).toBeInTheDocument();

    fireEvent.click(deliverHereBtn);

    const continueButton = await screen.findByTestId("buynow-btn");
    expect(continueButton).toBeInTheDocument();

    fireEvent.click(continueButton);

    const cancelOrder = await waitFor(() => screen.findByTestId("cancel-btn"), {
      timeout: 3000,
    });

    expect(cancelOrder).toBeInTheDocument();

    fireEvent.click(cancelOrder);

    const swalConfirmation = await screen.findByText(/Yes/i);
    expect(swalConfirmation).toBeInTheDocument();

    fireEvent.click(swalConfirmation);
  });

  test("clicks on buy now button and  get error responses", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressListData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.post("https://localhost:8080/api/Order", (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );

    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);

    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Stejin Jacob/i);
    expect(name).toBeInTheDocument();

    const deliverHereBtn = await screen.findByTestId("deliverHere");
    expect(deliverHereBtn).toBeInTheDocument();

    fireEvent.click(deliverHereBtn);

    const continueButton = await screen.findByTestId("buynow-btn");
    expect(continueButton).toBeInTheDocument();

    fireEvent.click(continueButton);

    const cancelOrder = await screen.findByTestId("cancel-btn");
    expect(cancelOrder).toBeInTheDocument();

    fireEvent.click(cancelOrder);

    const swalConfirmation = await screen.findByText(/Yes/i);
    expect(swalConfirmation).toBeInTheDocument();

    fireEvent.click(swalConfirmation);
  });

  test("clicks on buy now button and  get success responses then cancel order and get err response", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressListData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address?deliveryAddressId=2",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );

    server.use(
      rest.post("https://localhost:8080/api/Order", (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(createOrderResponse));
      })
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/Order/cancel?orderNumber=order",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );

    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);

    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();
    const name = await screen.findByText(/Stejin Jacob/i);
    expect(name).toBeInTheDocument();

    const deliverHereBtn = await screen.findByTestId("deliverHere");
    expect(deliverHereBtn).toBeInTheDocument();

    fireEvent.click(deliverHereBtn);

    const continueButton = await screen.findByTestId("buynow-btn");
    expect(continueButton).toBeInTheDocument();

    fireEvent.click(continueButton);

    const cancelOrder = await screen.findByTestId("cancel-btn");
    expect(cancelOrder).toBeInTheDocument();

    fireEvent.click(cancelOrder);

    const swalConfirmation = await screen.findByText(/Yes/i);
    expect(swalConfirmation).toBeInTheDocument();

    fireEvent.click(swalConfirmation);
  });

  test("should render checkout component without product id to get cart products and get error response on cart and address apis", async () => {
    server.use(
      rest.get("https://localhost:8080/api/Cart", (req, res, ctx) => {
        return res(ctx.status(400), ctx.json(getCartData));
      })
    );

    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );
    localStorage.setItem("orderCreated", true);
    render(<CheckoutWrapper />);
    const checkoutElement = screen.getByTestId("checkoutpage");
    expect(checkoutElement).toBeDefined();

    const noAddressFouund = await screen.findByText(/No saved address found/i);
    expect(noAddressFouund).toBeInTheDocument();
  });
});
