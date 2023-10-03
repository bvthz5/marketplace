import React, { useMemo, useState } from "react";
import { render, fireEvent, screen } from "@testing-library/react";
import { useNavigate } from "react-router-dom";
import UserDropdown from "../UserDropdown";
import { cartCounts } from "../../../../../App";
import { store } from "../../../../../redux/store";
import { Provider } from "react-redux";

const initialCartCount = 0;
const UserDropdownWrapper = ({ setLoggedIn }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <Provider store={store}>
      <cartCounts.Provider value={cartCountValue}>
        <UserDropdown setLoggedIn={setLoggedIn} />
      </cartCounts.Provider>
    </Provider>
  );
};

window.scrollTo = jest.fn();

jest.mock("react-router-dom", () => ({
  useNavigate: jest.fn(),
}));

test("should render dropdowncomponent component", () => {
  const setLoggedIn = jest.fn();

  render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
  const dropDownElement = screen.getByTestId("dropdowncomponent");
  expect(dropDownElement).toBeDefined();
});

describe("UserDropdown", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("should render without errors", () => {
    const setLoggedIn = jest.fn();
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
  });

  it("should display dropdown menu when button is clicked", () => {
    const setLoggedIn = jest.fn();
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
    const button = screen.getByTestId("dropdown-button");

    expect(button).toBeInTheDocument();

    fireEvent.click(button);
    const menu = screen.getByTestId("dropdown-menu");
    expect(menu).toBeInTheDocument();
  });

  it("should close the dropdown when user clicks outside of the component", () => {
    const setLoggedIn = jest.fn();
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
    const dropdownButton = screen.getByTestId("dropdown-button");
    fireEvent.click(dropdownButton);
    const dropdownMenu = screen.getByTestId("dropdown-menu");
    expect(dropdownMenu).toBeInTheDocument();

    fireEvent.mouseDown(document.body);
    expect(dropdownMenu).toBeInTheDocument();
  });

  it("should navigate to My Profile page when My profile is clicked", () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);

    fireEvent.click(screen.getByTestId("dropdown-button"));
    fireEvent.click(screen.getByTestId("myprofile"));
    expect(navigate).toHaveBeenCalledWith("/profile");
  });

  it("should navigate to My ads page when My ads is clicked", () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);
    localStorage.setItem("role", "2");
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
    fireEvent.click(screen.getByTestId("dropdown-button"));
    fireEvent.click(screen.getByTestId("myads"));
    expect(navigate).toHaveBeenCalledWith("/myproducts/?id=0");
  });

  it("should navigate to Wishlist page when Wishlist is clicked", () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);

    fireEvent.click(screen.getByTestId("dropdown-button"));
    fireEvent.click(screen.getByTestId("wishlist"));
    expect(navigate).toHaveBeenCalledWith("/wishlist");
  });

  it("should navigate to My Orders page when My Orders is clicked", () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);
    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);

    fireEvent.click(screen.getByTestId("dropdown-button"));
    fireEvent.click(screen.getByTestId("myorders"));
    expect(navigate).toHaveBeenCalledWith("/orders");
  });

  it("should log out and clear local storage when Logout is clicked", () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);

    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);

    fireEvent.click(screen.getByTestId("dropdown-button"));
    fireEvent.click(screen.getByText("Logout"));
    expect(screen.getByText("Logout?")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Yes, Logout!"));
    expect(navigate).toHaveBeenCalledTimes(0);
  });
});

describe("UserDropdown Component", () => {
  it("closes the menu when the Escape key is pressed", async () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);

    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);

    const button = screen.getByTestId("dropdown-button");
    fireEvent.click(button);

    const menu = await screen.findByTestId("dropdown-menu");

    fireEvent.keyDown(menu, { key: "Escape" });

    await expect(menu).not.toBeVisible();
  });

  it("prevents the default action wehen Tab key is pressed", async () => {
    const setLoggedIn = jest.fn();
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);

    render(<UserDropdownWrapper setLoggedIn={setLoggedIn} />);
    const button = screen.getByTestId("dropdown-button");
    fireEvent.click(button);

    const menu = await screen.findByTestId("dropdown-menu");

    fireEvent.keyDown(menu, { key: "Tab" });

    await expect(menu).not.toBeVisible();
  });
});
