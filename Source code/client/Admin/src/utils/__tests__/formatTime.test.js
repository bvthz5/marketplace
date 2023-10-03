import {fDate} from './../formatTime'
describe('fDate', () => {
  test('returns empty string if date is not provided', () => {
    expect(fDate()).toBe('');
  });

  test('returns formatted date string', () => {
    const date = '2022-05-01T00:00:00Z';
    expect(fDate(date)).toBe('01 May 2022');
  });

  test('returns formatted date string using provided format', () => {
    const date = '2022-05-01T00:00:00Z';
    const format = 'yyyy/MM/dd';
    expect(fDate(date, format)).toBe('2022/05/01');
  });

  test('returns default format if no format is provided', () => {
    const date = '2022-05-01T00:00:00Z';
    expect(fDate(date)).toBe('01 May 2022');
  });
});