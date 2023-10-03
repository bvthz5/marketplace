import React, { useMemo, useState } from "react";
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import AddressForm from "../AddressForm";
import { cartCounts } from "../../../../../../App";
import { addressEditData } from "../testData/data";
import { rest, server } from "../../../../../../testServer";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { store } from "../../../../../../redux/store";

window.scrollTo = jest.fn();

const initialCartCount = 0;
const AddressFormWrapper = ({ handleAddAddressDiv, editId }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <Router>
        <cartCounts.Provider value={cartCountValue}>
          <AddressForm
            handleAddAddressDiv={handleAddAddressDiv}
            editId={editId}
          />
        </cartCounts.Provider>
      </Router>
    </Provider>
  );
};

// =============================testing address add======================================

describe("add address", () => {
  test("should render addressform component and add invalid values to input fields", async () => {
    const handleAddAddressDiv = jest.fn();
    const user = userEvent.setup();

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={null}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), "Stejin");
    userEvent.clear(screen.getByTestId(/name-input/i));
    expect(screen.getByTestId(/name-input/i)).toHaveValue("");

    await user.type(screen.getByTestId(/phone-input/i), "123");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("123");

    await user.type(screen.getByTestId(/address-input/i), "Stejin~");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin~");

    await user.type(screen.getByTestId(/city-input/i), "ekm~");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm~");

    await user.type(screen.getByTestId(/state-input/i), "ker~");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("ker~");

    await user.type(screen.getByTestId(/street-input/i), "ekm~");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm~");

    await user.type(screen.getByTestId(/zipcode-input/i), "123");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
  });

  test("should render addressform component and add valid values to input fields and submit,get success response", async () => {
    const handleAddAddressDiv = jest.fn();

    const user = userEvent.setup();

    server.use(
      rest.post(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(200));
        }
      )
    );

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={null}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), "Stejin");
    expect(screen.getByTestId(/name-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/phone-input/i), "1234567890");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("1234567890");

    await user.type(screen.getByTestId(/address-input/i), "Stejin");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/city-input/i), "ekm");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/state-input/i), "ker");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("ker");

    await user.type(screen.getByTestId(/street-input/i), "ekm");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/zipcode-input/i), "123456");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123456");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
    fireEvent.click(submitButton);
  });

  test("should render addressform component and add valid values to input fields and submit,get error response(Address limit exceeded)", async () => {
    const handleAddAddressDiv = jest.fn();
    const user = userEvent.setup();

    server.use(
      rest.post(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(
            ctx.status(400),
            ctx.json({ message: "Address limit exceeded" })
          );
        }
      )
    );

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={null}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), "Stejin");
    expect(screen.getByTestId(/name-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/phone-input/i), "1234567890");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("1234567890");

    await user.type(screen.getByTestId(/address-input/i), "Stejin");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/city-input/i), "ekm");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/state-input/i), "ker");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("ker");

    await user.type(screen.getByTestId(/street-input/i), "ekm");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/zipcode-input/i), "123456");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123456");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
    fireEvent.click(submitButton);
  });
  test("should render addressform component and add valid values to input fields and submit,get error response(any message)", async () => {
    const handleAddAddressDiv = jest.fn();
    const user = userEvent.setup();

    server.use(
      rest.post(
        "https://localhost:8080/api/delivery-address",
        (req, res, ctx) => {
          return res(ctx.status(400), ctx.json({ message: "error" }));
        }
      )
    );

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={null}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), "Stejin");
    expect(screen.getByTestId(/name-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/phone-input/i), "1234567890");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("1234567890");

    await user.type(screen.getByTestId(/address-input/i), "Stejin");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin");

    await user.type(screen.getByTestId(/city-input/i), "ekm");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/state-input/i), "ker");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("ker");

    await user.type(screen.getByTestId(/street-input/i), "ekm");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm");

    await user.type(screen.getByTestId(/zipcode-input/i), "123456");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123456");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
    fireEvent.click(submitButton);
  });
});

// =============================testing address edit======================================
describe("edit address", () => {
  test("get success response on fetching address of edit id and submit the updated address and get success  response", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address/3",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressEditData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address/edit?deliveryAddressId=3",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json({ status: true }));
        }
      )
    );
    const handleAddAddressDiv = jest.fn();
    const user = userEvent.setup();

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={3}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), " jacob");
    expect(screen.getByTestId(/name-input/i)).toHaveValue("Stejin jacob");

    userEvent.clear(screen.getByTestId(/phone-input/i));
    await user.type(screen.getByTestId(/phone-input/i), "1234567890");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("1234567890");

    await user.type(screen.getByTestId(/address-input/i), " jacob");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin jacob");

    await user.type(screen.getByTestId(/city-input/i), " town");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm town");

    await user.type(screen.getByTestId(/state-input/i), "ala");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("kerala");

    await user.type(screen.getByTestId(/street-input/i), " town");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm town");

    userEvent.clear(screen.getByTestId(/zipcode-input/i));
    await user.type(screen.getByTestId(/zipcode-input/i), "123456");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123456");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
    fireEvent.click(submitButton);
  });

  test("get error response on fetching address of edit id", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address/3",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );

    const handleAddAddressDiv = jest.fn();

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={3}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();
  });

  test("get success response on fetching address of edit id and submit the updated address and get error  response", async () => {
    server.use(
      rest.get(
        "https://localhost:8080/api/delivery-address/3",
        (req, res, ctx) => {
          return res(ctx.status(200), ctx.json(addressEditData));
        }
      )
    );

    server.use(
      rest.put(
        "https://localhost:8080/api/delivery-address/edit?deliveryAddressId=3",
        (req, res, ctx) => {
          return res(ctx.status(400));
        }
      )
    );
    const handleAddAddressDiv = jest.fn();
    const user = userEvent.setup();

    render(
      <AddressFormWrapper
        handleAddAddressDiv={handleAddAddressDiv}
        editId={3}
      />
    );
    const addressFormElement = screen.getByTestId("addressform");
    expect(addressFormElement).toBeDefined();

    await user.type(screen.getByTestId(/name-input/i), " jacob");
    expect(screen.getByTestId(/name-input/i)).toHaveValue("Stejin jacob");

    userEvent.clear(screen.getByTestId(/phone-input/i));
    await user.type(screen.getByTestId(/phone-input/i), "1234567890");
    expect(screen.getByTestId(/phone-input/i)).toHaveValue("1234567890");

    await user.type(screen.getByTestId(/address-input/i), " jacob");
    expect(screen.getByTestId(/address-input/i)).toHaveValue("Stejin jacob");

    await user.type(screen.getByTestId(/city-input/i), " town");
    expect(screen.getByTestId(/city-input/i)).toHaveValue("ekm town");

    await user.type(screen.getByTestId(/state-input/i), "ala");
    expect(screen.getByTestId(/state-input/i)).toHaveValue("kerala");

    await user.type(screen.getByTestId(/street-input/i), " town");
    expect(screen.getByTestId(/street-input/i)).toHaveValue("ekm town");

    userEvent.clear(screen.getByTestId(/zipcode-input/i));
    await user.type(screen.getByTestId(/zipcode-input/i), "123456");
    expect(screen.getByTestId(/zipcode-input/i)).toHaveValue("123456");

    const submitButton = screen.getByTestId("submitbutton");
    expect(submitButton).toBeInTheDocument();
    fireEvent.click(submitButton);
  });
});
