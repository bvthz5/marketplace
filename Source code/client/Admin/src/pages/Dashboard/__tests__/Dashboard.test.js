import React from "react";
import Dashboard from "../Dashboard";
import { render } from "@testing-library/react";
import { BrowserRouter as Router } from 'react-router-dom';

jest.mock("react-apexcharts")

describe('nav component', () => {
    test('render nav component', () => {
  
      render(
        <Router>
          <Dashboard/>
        </Router>
      );
    });
   
  });