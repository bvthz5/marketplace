import { getRelativeDateTime } from "../Utils";

describe("getRelativeDateTime", () => {
  it('should return "now" when given the current time', () => {
    const now = new Date();
    expect(getRelativeDateTime(now)).toBe("now");
  });

  it("should return the minutes ago when given a date within the last hour", () => {
    const anHourAgo = new Date(Date.now() - 30 * 60 * 1000);
    expect(getRelativeDateTime(anHourAgo)).toBe("30m ago");
  });

  it('should return "1hr ago" when given a date exactly 1 hour ago', () => {
    const anHourAgo = new Date(Date.now() - 60 * 60 * 1000);
    expect(getRelativeDateTime(anHourAgo)).toBe("1hr ago");
  });

  it("should return the hours ago when given a date within the last day", () => {
    const yesterday = new Date(Date.now() - 12 * 60 * 60 * 1000);
    expect(getRelativeDateTime(yesterday)).toBe("12hr ago");
  });

  it('should return "yesterday" when given a date exactly 1 day ago', () => {
    const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000);
    expect(getRelativeDateTime(yesterday)).toBe("yesterday");
  });

  it("should return the month and day when given a date more than 1 day ago", () => {
    const twoDaysAgo = new Date(Date.now() - 2 * 24 * 60 * 60 * 1000);
    expect(getRelativeDateTime(twoDaysAgo)).toMatch(/[A-Z]{3} \d{1,2}/);
  });
});
