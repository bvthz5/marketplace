import Button from '../Button';

describe('Button', () => {
  it('should return a style object with overrides for MuiButton', () => {
    const theme = {
      palette: {
        grey: {
          500: '#999999',
          400: '#cccccc',
          800: '#333333',
        },
        action: {
          hover: '#eeeeee',
        },
      },
      customShadows: {
        z8: '0px 2px 4px rgba(0, 0, 0, 0.25)',
        primary: '0px 2px 4px rgba(255, 0, 0, 0.25)',
        secondary: '0px 2px 4px rgba(0, 255, 0, 0.25)',
      },
    };

    const style = Button(theme);

    expect(style).toEqual({
      MuiButton: {
        styleOverrides: {
          root: {
            '&:hover': {
              boxShadow: 'none',
            },
          },
          sizeLarge: {
            height: 48,
          },
          containedInherit: {
            color: '#333333',
            boxShadow: '0px 2px 4px rgba(0, 0, 0, 0.25)',
            '&:hover': {
              backgroundColor: '#cccccc',
            },
          },
          containedPrimary: {
            boxShadow: '0px 2px 4px rgba(255, 0, 0, 0.25)',
          },
          containedSecondary: {
            boxShadow: '0px 2px 4px rgba(0, 255, 0, 0.25)',
          },
          outlinedInherit: {
            border: '1px solid rgba(153, 153, 153, 0.32)',
            '&:hover': {
              backgroundColor: '#eeeeee',
            },
          },
          textInherit: {
            '&:hover': {
              backgroundColor: '#eeeeee',
            },
          },
        },
      },
    });
  });
});
