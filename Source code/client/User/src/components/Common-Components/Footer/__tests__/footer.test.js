import { render, screen } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import React from 'react';
import Footer from "../Footer";


test("should render footer component", () => {
    render( <Router><Footer /></Router>);
    const footerElement = screen.getByTestId("footer");
    expect(footerElement).toBeDefined();
  });