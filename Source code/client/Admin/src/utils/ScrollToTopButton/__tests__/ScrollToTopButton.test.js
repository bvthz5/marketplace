import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import ScrollToTopButton from "../ScrollToTopButton";

describe("ScrollToTopButton", () => {
  beforeAll(() => {
    // Mock the window.scrollTo method
    window.scrollTo = jest.fn();
  });

  it("should not render the button initially", () => {
    render(<ScrollToTopButton />);
    const button = screen.queryByTestId("scrolltotopbuttonpage");
    expect(button).toBeInTheDocument();
  });
  it("should render the button when scrolled past 500px", () => {
    render(<ScrollToTopButton />);
    window.scrollY = 1000;
    fireEvent.scroll(window);
    const button = screen.getByTestId("scrolltotopbuttonpage");
    expect(button).toBeInTheDocument();
  });

  it("should scroll to top when clicked", () => {
    render(<ScrollToTopButton />);
    window.scrollY = 1000;
    fireEvent.scroll(window);
    const button = screen.getByTitle("Go To Top");
    fireEvent.click(button);
    expect(window.scrollTo).toHaveBeenCalledWith({
      top: 0,
      behavior: "smooth",
    });
  });
});
