import { hasDataChanged } from "../Utils";

describe("hasDataChanged", () => {
  it("should return false when userData1 and userData2 are identical", () => {
    const userData1 = { name: "Alice", age: 30 };
    const userData2 = { name: "Alice", age: 30 };
    expect(hasDataChanged(userData1, userData2)).toBe(false);
  });

  it("should return true when userData1 and userData2 have different values for one key", () => {
    const userData1 = { name: "Alice", age: 30 };
    const userData2 = { name: "Alice", age: 31 };
    expect(hasDataChanged(userData1, userData2)).toBe(true);
  });

  it("should return true when userData1 and userData2 have different values for multiple keys", () => {
    const userData1 = { name: "Alice", age: 30, email: "alice@example.com" };
    const userData2 = { name: "Bob", age: 31, email: "bob@example.com" };
    expect(hasDataChanged(userData1, userData2)).toBe(true);
  });

});
