import Tooltip from '../Tooltip';

describe('Tooltip function', () => {
  it('should override styles for MuiTooltip tooltip and arrow', () => {
    const theme = {
      palette: {
        grey: {
          800: '#333',
        },
      },
    };
    const result = Tooltip(theme);
    const tooltipStyle = result.MuiTooltip.styleOverrides.tooltip;
    const arrowStyle = result.MuiTooltip.styleOverrides.arrow;

    expect(tooltipStyle.backgroundColor).toBe('#333');
    expect(arrowStyle.color).toBe('#333');
  });
});
