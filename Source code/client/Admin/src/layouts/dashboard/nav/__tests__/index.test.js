import React from 'react';
import { render } from '@testing-library/react';
import Nav from '../index';
import { BrowserRouter as Router } from 'react-router-dom';

describe('nav component', () => {
  test('render nav component', () => {
    const onCloseNav=jest.fn()
    render(
      <Router>
        <Nav openNav={true} onCloseNav={onCloseNav} />
      </Router>
    );
  });
  test('render nav with opennav false', () => {
 
    render(
      <Router>
        <Nav openNav={false}  />
      </Router>
    );
  });
});
