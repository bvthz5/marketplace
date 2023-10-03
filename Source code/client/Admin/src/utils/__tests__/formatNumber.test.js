import {fNumber}from './../formatNumber'
describe('fNumber', () => {
    test('returns formatted number string', () => {
      expect(fNumber(1000)).toBe('1,000');
    });
  
    test('returns formatted number string for negative number', () => {
      expect(fNumber(-1000)).toBe('-1,000');
    });
  
    test('returns formatted number string for decimal number', () => {
      expect(fNumber(1235)).toBe('1,235');
    });
  });
  


