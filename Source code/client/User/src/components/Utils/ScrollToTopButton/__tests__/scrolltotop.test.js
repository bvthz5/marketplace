
import { fireEvent, render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import ScrollToTopButton from "../ScrollToTopButton";

window.scrollTo = jest.fn();

test("should render button after scrolling down", async () => {
  render(
    <Router>
      <ScrollToTopButton />
    </Router>
  );

  const scrollToTopButtonElement = screen.getByTestId("scrolltotopbtn");
  expect(scrollToTopButtonElement).toBeDefined();

  window.scrollY = 500;
  fireEvent.scroll(window);

  const goToTopButton = await screen.findByTestId("go-to-top-btn");
  expect(goToTopButton).toBeInTheDocument();
});

test("should scroll to top when button is clicked", async () => {
  render(
    <Router>
      <ScrollToTopButton />
    </Router>
  );
  window.scrollTo = jest.fn();
  window.scrollY = 500;
  fireEvent.scroll(window);

  const goToTopButton = await screen.findByTestId("go-to-top-btn");
  expect(goToTopButton).toBeInTheDocument();

  fireEvent.click(goToTopButton);
  expect(window.scrollTo).toHaveBeenCalledWith({
    top: 0,
    behavior: "smooth",
  });
});

test("shouldn't render button when user scrolls back to top", async () => {
  render(
    <Router>
      <ScrollToTopButton />
    </Router>
  );
  window.scrollTo = jest.fn();
  window.scrollY = 500;
  fireEvent.scroll(window);

  const goToTopButton = await screen.findByTestId("go-to-top-btn");
  expect(goToTopButton).toBeInTheDocument();

  window.scrollY = 200;
  fireEvent.scroll(window);

  expect(goToTopButton).not.toBeInTheDocument();
});
