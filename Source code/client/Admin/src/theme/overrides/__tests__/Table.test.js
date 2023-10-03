import Table from '../Table';

describe('Table function', () => {
  it('should override styles for MuiTableCell head', () => {
    const theme = {
      palette: {
        text: {
          secondary: '#999',
        },
        background: {
          neutral: '#ccc',
        },
      },
    };
    const result = Table(theme);
    const tableCellHeadStyle = result.MuiTableCell.styleOverrides.head;

    expect(tableCellHeadStyle.color).toBe('#999');
    expect(tableCellHeadStyle.backgroundColor).toBe('#ccc');
  });
});
