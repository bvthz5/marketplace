import Typography from '../Typography';

describe('Typography function', () => {
  it('should override styles for MuiTypography paragraph and gutterBottom', () => {
    const theme = {
      spacing: (factor) => `${factor}rem`,
    };
    const result = Typography(theme);
    const paragraphStyle = result.MuiTypography.styleOverrides.paragraph;
    const gutterBottomStyle = result.MuiTypography.styleOverrides.gutterBottom;

    expect(paragraphStyle.marginBottom).toBe('2rem');
    expect(gutterBottomStyle.marginBottom).toBe('1rem');
  });
});
