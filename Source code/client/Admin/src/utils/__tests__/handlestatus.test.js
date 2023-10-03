import { handleStatus } from '../utils';


describe('handleStatus function', () => {
  it('should return "Rejected" when status is 0', () => {
    const status = 0;
    const result = handleStatus(status);
    expect(result.props.className).toBe('logged-rejected');
    expect(result.props.children).toBe('Rejected');
  });

  it('should return "Approved" when status is 1', () => {
    const status = 1;
    const result = handleStatus(status);
    expect(result.props.className).toBe('logged-in');
    expect(result.props.children).toBe('Approved');
  });

  it('should return "Pending" when status is 2', () => {
    const status = 2;
    const result = handleStatus(status);
    expect(result.props.className).toBe('warning');
    expect(result.props.children).toBe('Pending');
  });

  it('should return "Sold" when status is 3', () => {
    const status = 3;
    const result = handleStatus(status);
    expect(result.props.className).toBe('logged-sold');
    expect(result.props.children).toBe('Sold');
  });

  it('should return "Deleted" when status is 4', () => {
    const status = 4;
    const result = handleStatus(status);
    expect(result.props.className).toBe('logged-out');
    expect(result.props.children).toBe('Deleted');
  });

  it('should return "Order Processing" when status is 6', () => {
    const status = 6;
    const result = handleStatus(status);
    expect(result.props.className).toBe('warning');
    expect(result.props.children).toBe('Order Processing');
  });

});
