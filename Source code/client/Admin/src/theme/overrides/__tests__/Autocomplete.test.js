import Autocomplete from './../Autocomplete';

describe('Autocomplete', () => {
  it('should return the expected style overrides for MuiAutocomplete', () => {
    const theme = {
      customShadows: {
        z20: '0px 0px 20px rgba(0, 0, 0, 0.1)',
      },
    };

    const overrides = Autocomplete(theme);
    const expectedBoxShadow = theme.customShadows.z20;
    const actualBoxShadow = overrides.MuiAutocomplete.styleOverrides.paper.boxShadow;

    expect(actualBoxShadow).toBe(expectedBoxShadow);
  });
});
