
import React, { useMemo, useState } from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import { cartCounts } from "../../../../App";
import Title from "../Title";
import { useNavigate } from "react-router-dom";

const initialCartCount = 0;
const TitleWrapper = ({ pageTitle }) => {
  const [cartCount, setCartCount] = useState(initialCartCount);
  const cartCountValue = useMemo(() => [cartCount, setCartCount], [cartCount]);
  return (
    <cartCounts.Provider value={cartCountValue}>
      <Title pageTitle={pageTitle} />
    </cartCounts.Provider>
  );
};

jest.mock("react-router-dom", () => ({
  useNavigate: jest.fn(),
}));

describe("Title component", () => {
  let pageTitle = "Test Page";
  let navigate = jest.fn();

  beforeAll(() => {
    useNavigate.mockReturnValue(navigate);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it("renders the component with the correct title", () => {
    render(<TitleWrapper pageTitle={pageTitle} />);
    expect(screen.getByText(pageTitle)).toBeInTheDocument();
  });

  it("calls goBack function when back button is clicked", () => {
    render(<TitleWrapper pageTitle={pageTitle} />);
    fireEvent.click(screen.getByTestId("gobackbutton"));
    expect(navigate).toHaveBeenCalledWith(-1);
  });
});
