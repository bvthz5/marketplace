import { getRelativeDate } from "../Utils";

describe("getRelativeDate", () => {
  it('returns "TODAY" for today\'s date', () => {
    const today = new Date();
    const expected = "TODAY";
    expect(getRelativeDate(today)).toEqual(expected);
  });

  it('returns "YESTERDAY" for yesterday\'s date', () => {
    const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000);
    const expected = "YESTERDAY";
    expect(getRelativeDate(yesterday)).toEqual(expected);
  });

  it('returns "X DAYS AGO" for a date less than a week ago', () => {
    const threeDaysAgo = new Date(Date.now() - 3 * 24 * 60 * 60 * 1000);
    const expected = "3 DAYS AGO";
    expect(getRelativeDate(threeDaysAgo)).toEqual(expected);
  });
  
  it("returns correct relative date for a date ten days ago", () => {
    const timezoneOffset = new Date().getTimezoneOffset();
    const tenDaysAgo = new Date(
      Date.now() - 10 * 24 * 60 * 60 * 1000 + timezoneOffset * 60 * 1000
    );
    const expected = getRelativeDate(tenDaysAgo);
    expect(getRelativeDate(tenDaysAgo)).toEqual(expected);
  });
});



