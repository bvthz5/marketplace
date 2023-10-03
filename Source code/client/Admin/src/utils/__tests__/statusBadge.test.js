import { statusBadge } from '../utils';
import { render, screen } from '@testing-library/react';

describe('statusBadge', () => {
  it('should render the correct badge for each status', () => {
    render(statusBadge(3));
    expect(screen.getByText('Deleted')).toBeInTheDocument();
    expect(screen.getByText('Deleted')).toHaveStyle('color: red');
    expect(screen.getByText('Deleted')).toHaveStyle('background-color: rgba(255, 86, 48, 0.16)');

    render(statusBadge(2));
    expect(screen.getByText('Blocked')).toBeInTheDocument();
    expect(screen.getByText('Blocked')).toHaveStyle('color: orange');
    expect(screen.getByText('Blocked')).toHaveStyle('background-color: #e9981629');

    render(statusBadge(1));
    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Active')).toHaveStyle('color: green');
    expect(screen.getByText('Active')).toHaveStyle('background-color: rgba(7, 92, 49, 0.16)');

    render(statusBadge(0));
    expect(screen.getByText('Inactive')).toBeInTheDocument();
    expect(screen.getByText('Inactive')).toHaveStyle('color: #4a63ee');
    expect(screen.getByText('Inactive')).toHaveStyle('background-color: #0f1ef129');
  });

  it('should render the badge for the default case', () => {
    render(statusBadge(999));
    expect(screen.getByText('Inactive')).toBeInTheDocument();
    expect(screen.getByText('Inactive')).toHaveStyle('color: #4a63ee');
    expect(screen.getByText('Inactive')).toHaveStyle('background-color: #0f1ef129');
  });
});
