import "@testing-library/jest-dom/extend-expect";
import { render, screen,waitFor,fireEvent } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import { rest, server } from '../../../testServer';
import Topbar from '../Topbar';
import { categoryDataTopbar } from '../testData/data';
import userEvent from "@testing-library/user-event";


test('should render topbar component', () => {

    const setCategory = jest.fn();
    const setStartRange = jest.fn();
    const setEndRange = jest.fn();
    const setSort = jest.fn();
    const setStatus = jest.fn();
    const clearFilters = jest.fn();
  render(
      <Router>
        <Topbar
         setStartRange={setStartRange}
         setCategory={setCategory}
         setEndRange={setEndRange}
         setSort={setSort}
         setStatus={setStatus}
         clearFilters={clearFilters}
        />
      </Router>
  );
  const TopbarElement = screen.getByTestId('topbarpage');
  expect( TopbarElement).toBeDefined();


});

test("change product category sort , price sort dropdowns  and  status change filter", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(categoryDataTopbar));
    })
  );
  const setCategory = jest.fn();
  const setStartRange = jest.fn();
  const setEndRange = jest.fn();
  const setSort = jest.fn();
  const clearFilters = jest.fn();
  const setStatus=jest.fn();
  const setDesc=jest.fn();

  render(
    <Topbar
      setStartRange={setStartRange}
      setCategory={setCategory}
      setEndRange={setEndRange}
      setSort={setSort}
      clearFilters={clearFilters}
      setStatus={setStatus}
      desc={setDesc}
    />
  );

  // CHECK IF SELECT DROPDOWN EXISTS
  const priceSort = await waitFor(
    () => screen.findByTestId("price-sort-dropdown"),
    {
      timeout: 3000,
    }
  );
  expect(priceSort).toBeInTheDocument();

  fireEvent.change(priceSort, {
    target: { value: "Price:high to low" },
  });

  priceSort.dispatchEvent(new Event("change"));

  const categorySort = await waitFor(
    () => screen.findByTestId("category-sort-dropdown"),
    {
      timeout: 3000,
    }
  );
  expect(categorySort).toBeInTheDocument();

  fireEvent.change(categorySort, {
    target: { value: "Camera" },
  });

  categorySort.dispatchEvent(new Event("change"));


  const statusfilter = await waitFor(
    () => screen.findByTestId("status-filter-dropdown"),
    {
      timeout: 3000,
    }
  );
  expect(statusfilter).toBeInTheDocument();

  fireEvent.change(statusfilter, {
    target: { value: "Active" },
  });

  statusfilter.dispatchEvent(new Event("change"));
});

test("enter diffrent price values to cover error messages", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(categoryDataTopbar));
    })
  );
  const setCategory = jest.fn();
  const setStartRange = jest.fn();
  const setEndRange = jest.fn();
  const setSort = jest.fn();
  const clearFilters = jest.fn();

  const user = userEvent.setup();
  render(
    <Topbar
      setStartRange={setStartRange}
      setCategory={setCategory}
      setEndRange={setEndRange}
      setSort={setSort}
      clearFilters={clearFilters}
    />
  );
  const TopbarElement = screen.getByTestId('topbarpage');
  expect( TopbarElement).toBeDefined();

  await user.type(screen.getByTestId("start-price"), "e1000");
  expect(screen.getByTestId("start-price")).toHaveValue(1000);

  await user.type(screen.getByTestId("end-price"), "1000");
  expect(screen.getByTestId("end-price")).toHaveValue(1000);

  await user.type(screen.getByTestId("start-price"), "0");
  expect(screen.getByTestId("start-price")).toHaveValue(10000);

  await user.type(screen.getByTestId("end-price"), "e000");
  expect(screen.getByTestId("end-price")).toHaveValue(1000000);
});
test("enter diffrent price values to cover clear button", async () => {
  server.use(
    rest.get("https://localhost:8080/api/Category", (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(categoryDataTopbar));
    })
  );
  const setCategory = jest.fn();
  const setStartRange = jest.fn();
  const setEndRange = jest.fn();
  const setSort = jest.fn();
  const clearFilters = jest.fn();

  const user = userEvent.setup();
  render(
    <Topbar
      setStartRange={setStartRange}
      setCategory={setCategory}
      setEndRange={setEndRange}
      setSort={setSort}
      clearFilters={clearFilters}
    />
  );
  const TopbarElement = screen.getByTestId('topbarpage');
  expect( TopbarElement).toBeDefined();

  await user.type(screen.getByTestId("start-price"), "e1000");
  expect(screen.getByTestId("start-price")).toHaveValue(1000);

  await user.type(screen.getByTestId("end-price"), "1000");
  expect(screen.getByTestId("end-price")).toHaveValue(1000);

  await user.type(screen.getByTestId("start-price"), "0");
  expect(screen.getByTestId("start-price")).toHaveValue(10000);

  await user.type(screen.getByTestId("end-price"), "e000");
  expect(screen.getByTestId("end-price")).toHaveValue(1000000);
  

  const clearbutton=screen.getByTestId('clearbutton');
  expect (clearbutton).toBeDefined();
  fireEvent.click(clearbutton);
});