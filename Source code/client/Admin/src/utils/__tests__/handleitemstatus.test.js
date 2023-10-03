import { handleItemStatus } from '../utils';
import { orderStatus} from '../Enums';

describe('handleItemStatus', () => {
  it('should return the correct status message for each status', () => {
    expect(handleItemStatus(orderStatus.CREATED)).toBe('Order created');
    expect(handleItemStatus(orderStatus.CONFIRMED)).toBe('Order Confirmed');
    expect(handleItemStatus(orderStatus.INTRANSIT)).toBe('In Transit');
    expect(handleItemStatus(orderStatus.CANCELLED)).toBe('Order Cancelled');
    expect(handleItemStatus(orderStatus.WAITING_FOR_PICKUP)).toBe('Waiting For Pickup');
    expect(handleItemStatus(orderStatus.OUTFORDELIVERY)).toBe('Out For Delivery');
    expect(handleItemStatus(orderStatus.DELIVERED)).toBe('Delivered');
  });

  it('should return an empty string for an unknown status', () => {
    expect(handleItemStatus('UNKNOWN_STATUS')).toBe('');
  });
});