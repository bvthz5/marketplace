import { render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from "react";
import Lists from "../Lists";

test("should render lists component", () => {
  const handleNavigation = jest.fn();

  render(
    <Router>
      <Lists handleNavigation={handleNavigation} />
    </Router>
  );
  const listsElement = screen.getByTestId("listcomponent");
  expect(listsElement).toBeDefined();
});
