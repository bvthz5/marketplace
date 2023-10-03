import { getOrderStatuses } from "../Data";

describe('getOrderStatuses', () => {
    it('should return "Order Created" for status 0', () => {
      expect(getOrderStatuses(0)).toEqual('Order Created');
    });
    
    it('should return "Order Failed" for status 1', () => {
      expect(getOrderStatuses(1)).toEqual('Order Failed');
    });
  
    it('should return "Confirmed" for status 2', () => {
      expect(getOrderStatuses(2)).toEqual('Confirmed');
    });
  
    it('should return "Waiting For Pickup" for status 3', () => {
      expect(getOrderStatuses(3)).toEqual('Waiting For Pickup');
    });
  
    it('should return "In Transit" for status 4', () => {
      expect(getOrderStatuses(4)).toEqual('In Transit');
    });
  
    it('should return "Out For Delivery" for status 5', () => {
      expect(getOrderStatuses(5)).toEqual('Out For Delivery');
    });
  
    it('should return "Cancelled" for status 6', () => {
      expect(getOrderStatuses(6)).toEqual('Cancelled');
    });

    it('should return "Delivered" for status 7', () => {
      expect(getOrderStatuses(7)).toEqual('Delivered');
    });
  
    it('should return "Status Not Found" for an unknown status', () => {
      expect(getOrderStatuses(10)).toEqual('Status Not Found');
    });
  });