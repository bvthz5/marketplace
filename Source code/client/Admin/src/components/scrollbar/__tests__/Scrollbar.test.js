import React from 'react';
import { render, screen } from '@testing-library/react';
import Scrollbar from '../Scrollbar';

describe('Scrollbar', () => {
    test('renders children without error', () => {
      render(
        <Scrollbar>
          <div>Child 1</div>
          <div>Child 2</div>
          <div>Child 3</div>
        </Scrollbar>
      );
  
      const childNodes = screen.getAllByText(/child/i);
      expect(childNodes).toHaveLength(3);
    });
  });
