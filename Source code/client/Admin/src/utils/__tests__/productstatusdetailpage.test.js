import { render } from '@testing-library/react';
import { handleProductStatus } from '../utils';

describe('handleProductStatus', () => {
  test('returns "Rejected" for status 0', () => {
    const status = 0;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Rejected');
  });

  test('returns "Approved" for status 1', () => {
    const status = 1;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Approved');
  });

  test('returns "Pending" for status 2', () => {
    const status = 2;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Pending');
  });

  test('returns "Sold" for status 3', () => {
    const status = 3;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Sold');
  });

  test('returns "Deleted" for status 4', () => {
    const status = 4;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Deleted');
  });

  test('returns "Order Processing" for status 6', () => {
    const status = 6;
    const { container } = render(handleProductStatus(status));
    expect(container.textContent).toEqual('Order Processing');
  });
});
