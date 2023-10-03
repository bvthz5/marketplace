import Backdrop from './../Backdrop';

describe('Backdrop', () => {
  it('should return the expected style overrides for MuiBackdrop', () => {
    const theme = {
      palette: {
        grey: {
          800: '#424242',
        },
      },
    };

    const overrides = Backdrop(theme);

    expect(overrides).toMatchObject({
      MuiBackdrop: {
        styleOverrides: {
          root: {
            backgroundColor: 'rgba(66, 66, 66, 0.8)',
          },
          invisible: {
            background: 'transparent',
          },
        },
      },
    });
  });
});
