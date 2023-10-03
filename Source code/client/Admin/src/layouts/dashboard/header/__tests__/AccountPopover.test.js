import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import AccountPopover from '../AccountPopover';
jest.mock('../../../../assets/adminimage.jpg');
describe('nav component', () => {
  test('render nav component', () => {
    render(
      <Router>
        <AccountPopover />
      </Router>
    );
  });

  test('account popover open', async () => {
    render(
      <Router>
        <AccountPopover />
      </Router>
    );
    const accountPopover = screen.getByTestId('iconbutton');
    expect(accountPopover).toBeDefined();
    fireEvent.click(accountPopover);

    const logout=screen.getByText('Logout');
    expect(logout).toBeInTheDocument();
    fireEvent.click(logout)

    const logoutconfirm= await screen.findByText('Yes, Logout!');
    expect(logoutconfirm).toBeInTheDocument();

  });
});
