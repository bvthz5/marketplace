import Card from '../Card';

describe('Card', () => {
  it('should return the expected style overrides for MuiCard', () => {
    const theme = {
      customShadows: {
        card: '0px 14px 80px rgba(34, 35, 58, 0.2), 0px 10px 20px rgba(34, 35, 58, 0.15), 0px 6px 6px rgba(34, 35, 58, 0.1)',
      },
      shape: {
        borderRadius: 8,
      },
      spacing: (value) => value * 8,
    };

    const overrides = Card(theme);

    expect(overrides).toMatchObject({
      MuiCard: {
        styleOverrides: {
          root: {
            boxShadow: '0px 14px 80px rgba(34, 35, 58, 0.2), 0px 10px 20px rgba(34, 35, 58, 0.15), 0px 6px 6px rgba(34, 35, 58, 0.1)',
            borderRadius: 16,
            position: 'relative',
            zIndex: 0,
          },
        },
      },
      MuiCardHeader: {
        defaultProps: {
          titleTypographyProps: { variant: 'h6' },
          subheaderTypographyProps: { variant: 'body2' },
        },
        styleOverrides: {
          root: {
            padding: 24,
          },
        },
      },
      MuiCardContent: {
        styleOverrides: {
          root: {
            padding: 24,
          },
        },
      },
    });
  });
});
