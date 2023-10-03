import { convertDatesmall } from "../formatDate";

describe('convertDate', () => {
    it('returns the correct date string for a valid date', () => {
      const date = new Date('2022-01-01T00:00:00Z');
      expect(convertDatesmall(date)).toBe('1 Jan 2022');
    });
  
    it('returns an empty string for an invalid date', () => {
      const date = 'not a date';
      expect(convertDatesmall(date)).toBe('');
    });
  });