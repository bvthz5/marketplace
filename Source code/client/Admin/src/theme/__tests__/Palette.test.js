import palette from '../palette';

describe('palette', () => {
  test('primary color is a valid hex code', () => {
    expect(/^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/.test(palette.primary.main)).toBe(true);
  });

  test('background color includes neutral color', () => {
    expect(Object.values(palette.background)).toContain(palette.grey[200]);
  });

  test('text colors include primary, secondary, and disabled', () => {
    expect(Object.keys(palette.text)).toEqual(expect.arrayContaining(['primary', 'secondary', 'disabled']));
  });

  test('action hover opacity is 0.08', () => {
    expect(palette.action.hoverOpacity).toBe(0.08);
  });
});
