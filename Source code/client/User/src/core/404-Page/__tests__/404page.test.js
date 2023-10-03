import "@testing-library/jest-dom";
import { fireEvent, render, screen } from "@testing-library/react";
import { useNavigate } from "react-router-dom";
import React from "react";
import NotFound from "../NotFound";

jest.mock("react-router-dom", () => ({
  useNavigate: jest.fn(),
}));

test("should render notfound component", () => {
  render(<NotFound />);
  const notFoundElement = screen.getByTestId("notfoundpage");
  expect(notFoundElement).toBeDefined();
});

describe("NotFound", () => {
  test("renders not found page with image and helpful links", () => {
    const navigate = jest.fn();
    useNavigate.mockReturnValue(navigate);
    render(<NotFound />);

    // Test if the main elements of the not found page are rendered
    expect(screen.getByTestId("notfoundpage")).toBeInTheDocument();
    expect(screen.getByText("Oops!")).toBeInTheDocument();
    expect(screen.getByText("We can't seem to find that.")).toBeInTheDocument();
    expect(screen.getByText("Try searching for it.")).toBeInTheDocument();
    expect(screen.getByText("Error 404")).toBeInTheDocument();
    expect(screen.getByText("Home")).toBeInTheDocument();
    expect(screen.getByText("Go Back")).toBeInTheDocument();

    // Test clicking on helpful links navigates correctly
    fireEvent.click(screen.getByText("Home"));
    expect(navigate).toHaveBeenCalledWith("/home");
    fireEvent.click(screen.getByText("Go Back"));
    expect(navigate).toHaveBeenCalledWith(-1);
  });
});
