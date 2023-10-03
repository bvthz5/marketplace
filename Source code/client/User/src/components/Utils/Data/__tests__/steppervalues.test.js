import { stepperValues } from "../Data";

describe("stepperValues", () => {
  it('should contain an object with id 0 and value "Order Confirmed"', () => {
    expect(stepperValues).toContainEqual({ id: 0, value: "Order Confirmed", status: 2 });
  });

  it('should contain an object with id 1 and value "Waiting For Pickupd"', () => {
    expect(stepperValues).toContainEqual({ id: 1, value: "Waiting For Pickup", status: 3 });
  });

  it('should contain an object with id 2 and value "In Transit"', () => {
    expect(stepperValues).toContainEqual({ id: 2, value: "In Transit", status: 4 });
  });

  it('should contain an object with id 3 and value "Out For Delivery"', () => {
    expect(stepperValues).toContainEqual( { id: 3, value: "Out For Delivery", status: 5 });
  });

  it('should contain an object with id 4 and value "Delivered"', () => {
    expect(stepperValues).toContainEqual({ id: 4, value: "Delivered", status: 7 });
  });

  it("should have a length of 5", () => {
    expect(stepperValues).toHaveLength(5);
  });

  it("should not contain an object with id 5", () => {
    expect(stepperValues.some((item) => item.id === 5)).toBe(false);
  });
});
