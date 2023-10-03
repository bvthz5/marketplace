import { activeStepper } from "../Data";

describe("activeStepper", () => {
  it("should return 1 when order status is 2", () => {
    expect(activeStepper(2)).toBe(1);
  });

  it("should return 2 when order status is 3", () => {
    expect(activeStepper(3)).toBe(2);
  });

  it("should return 3 when order status is 4", () => {
    expect(activeStepper(4)).toBe(3);
  });

  it("should return 4 when order status is 5", () => {
    expect(activeStepper(5)).toBe(4);
  });

  it("should return 5 when order status is 7", () => {
    expect(activeStepper(7)).toBe(5);
  });

  it("should return 10 when order status is not recognized", () => {
    expect(activeStepper(10)).toBe(10);
  });
});
