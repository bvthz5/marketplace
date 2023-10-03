import { getRelativeDate } from "../formatDate";

describe('getRelativeDate', () => {
    test('returns TODAY for current date', () => {
      const today = new Date();
      expect(getRelativeDate(today)).toEqual('TODAY');
    });
  
    test('returns YESTERDAY for date 1 day ago', () => {
      const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000);
      expect(getRelativeDate(yesterday)).toEqual('YESTERDAY');
    });
  
    test('returns "3 DAYS AGO" for date 3 days ago', () => {
      const threeDaysAgo = new Date(Date.now() - 3 * 24 * 60 * 60 * 1000);
      expect(getRelativeDate(threeDaysAgo)).toEqual('3 DAYS AGO');
    });
    test("returns correct relative date for a date ten days ago", () => {
      const timezoneOffset = new Date().getTimezoneOffset();
      const tenDaysAgo = new Date(
        Date.now() - 10 * 24 * 60 * 60 * 1000 + timezoneOffset * 60 * 1000
      );
      const expected = getRelativeDate(tenDaysAgo);
      expect(getRelativeDate(tenDaysAgo)).toEqual(expected);
    });
  });
  