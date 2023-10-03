import { fireEvent,render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import UserDetails from '../UserDetail';
import { rest, server } from '../../../../testServer';
import { getUserDetailData } from '../testData/data';


jest.mock('./../../../assets/Screenshot-2018-12-16-at-21.06.29.png');
window.scrollTo = jest.fn();
describe("userdetail componenttest",()=>{
test('should render User details component', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/19', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getUserDetailData));
    })
  );
  render(
    <Router>
      <UserDetails seller={getUserDetailData.data} />
    </Router>
  );
  const userDetailsElement = screen.getByTestId('userdetailpage');
  expect(userDetailsElement).toBeDefined();

  const avatar=screen.getByTestId("avatarid")
  expect(avatar).toBeInTheDocument();
  fireEvent.click(avatar);

 


});
test('should render User details component with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/User/19', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
       <UserDetails seller={getUserDetailData.data} />
    </Router>
  );
  const userDetailsElement = screen.getByTestId('userdetailpage');
  expect(userDetailsElement).toBeDefined();

  

});
});