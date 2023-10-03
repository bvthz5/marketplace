import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import ProductList from '../ProductList';
import { rest, server } from '../../../testServer';
import { getProductListData } from '../testData/data';
import userEvent from '@testing-library/user-event';
import React from 'react';
import { categoryDataTopbar } from '../../Topbar/testData/data';

jest.mock('./../../assets/Image_not_available.png');
window.scrollTo = jest.fn();

const ProductlistWrapper = ({
  clearFilters,
  handleCategory,
  handleSort,
  setStartRange,
  setEndRange,
  handleSearch,
  addFavourites,
  deleteFavourites,
}) => {
  return (
    <Router>
      <ProductList
        setStartRange={setStartRange}
        setEndRange={setEndRange}
        setCategory={handleCategory}
        setSort={handleSort}
        clearFilters={clearFilters}
        handleSearch={handleSearch}
        addFavourites={addFavourites}
        deleteFavourites={deleteFavourites}
      />
    </Router>
  );
};

server.use(
  rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getProductListData));
  })
);
server.use(
  rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(categoryDataTopbar));
  })
);

describe('productlist component', () => {
  test('should render productlist component', () => {
    server.use(
      rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductListData));
      })
    );
    render(
      <Router>
        <ProductList />
      </Router>
    );
    const productListElement = screen.getByTestId('productlistpage');
    expect(productListElement).toBeDefined();
  });
  test('get product  with error response', () => {
    server.use(
      rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(
      <Router>
        <ProductList />
      </Router>
    );
    const productListElement = screen.getByTestId('productlistpage');
    expect(productListElement).toBeDefined();
  });
  test('render productlist and apply diffrent filters', async () => {
    server.use(
      rest.get('https://localhost:8080/api/Product/offset', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getProductListData));
      })
    );
    server.use(
      rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(categoryDataTopbar));
      })
    );
    
    localStorage.clear();
    const handleCategory = jest.fn();
    const handleSort = jest.fn();
    const setStartRange = jest.fn();
    const setEndRange = jest.fn();
    const clearFilters = jest.fn();
    const handleSearch = jest.fn();

    const user = userEvent.setup();

    render(
      <ProductlistWrapper
        setStartRange={setStartRange}
        setEndRange={setEndRange}
        setCategory={handleCategory}
        setSort={handleSort}
        clearFilters={clearFilters}
        handleSearch={handleSearch}
      />
    );
    const productListElement = screen.getByTestId('productlistpage');
    expect(productListElement).toBeDefined();

    const data = await screen.findByText(/Apple/i);
    expect(data).toBeInTheDocument();

    const card = await screen.findByTestId('product-card');
    expect(card).toBeInTheDocument();

    fireEvent.click(card);

    const clearAllButton = screen.getByTestId('clearbutton');
    expect(clearAllButton).toBeDefined();
    fireEvent.click(clearAllButton);

    await user.type(screen.getByTestId('start-price'), '1000');
    expect(screen.getByTestId('start-price')).toHaveValue(1000);

    await user.type(screen.getByTestId('end-price'), '1000');
    expect(screen.getByTestId('end-price')).toHaveValue(1000);

    // CHECK IF SELECT DROPDOWN EXISTS
    const priceSort = await waitFor(() => screen.findByTestId('price-sort-dropdown'), {
      timeout: 3000,
    });
    expect(priceSort).toBeInTheDocument();

    fireEvent.change(priceSort, {
      target: { value: 'Price:low to high' },
    });

    priceSort.dispatchEvent(new Event('change'));

    fireEvent.change(priceSort, {
      target: { value: 'Price:high to low' },
    });

    priceSort.dispatchEvent(new Event('change'));
    fireEvent.change(priceSort, {
      target: { value: 'Oldest to Newest' },
    });

    priceSort.dispatchEvent(new Event('change'));
    fireEvent.change(priceSort, {
      target: { value: 'Newest to Oldest' },
    });

    priceSort.dispatchEvent(new Event('change'));

    const categorySort = await waitFor(() => screen.findByTestId('category-sort-dropdown'), {
      timeout: 3000,
    });
    expect(categorySort).toBeInTheDocument();

    fireEvent.change(categorySort, {
      target: { value: 'Camera' },
    });

    categorySort.dispatchEvent(new Event('change'));

    await user.type(screen.getByTestId(/search-input/i), 'apple');
    expect(screen.getByTestId(/search-input/i)).toHaveValue('apple');

    categorySort.dispatchEvent(new Event('change'));

    const statusfilter = await waitFor(() => screen.findByTestId('status-filter-dropdown'), {
      timeout: 3000,
    });
    expect(statusfilter).toBeInTheDocument();

    fireEvent.change(statusfilter, {
      target: { value: 'Active' },
    });

    statusfilter.dispatchEvent(new Event('change'));
  });
});
