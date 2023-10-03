import Paper from '../Paper';

describe('Paper function', () => {
  it('should set defaultProps for MuiPaper', () => {
    const result = Paper();
    const paperProps = result.MuiPaper.defaultProps;
    expect(paperProps.elevation).toBe(0);
  });

  it('should override root styles for MuiPaper', () => {
    const result = Paper();
    const paperStyles = result.MuiPaper.styleOverrides.root;
    expect(paperStyles.backgroundImage).toBe('none');
  });
});
