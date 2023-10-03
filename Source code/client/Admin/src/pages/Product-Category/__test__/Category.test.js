import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom/extend-expect';
import { BrowserRouter as Router } from 'react-router-dom';
import userEvent from '@testing-library/user-event';
import React from 'react';
import Category from '../Category';
import { blockCategoryData, getCategoryData, unblockCategoryData } from '../testData/data';
import { rest, server } from '../../../testServer';

server.use(
  rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getCategoryData));
  })
);

test('should render Category component', () => {
  render(
    <Router>
      <Category />
    </Router>
  );
  const categoryElement = screen.getByTestId('categorypage');
  expect(categoryElement).toBeDefined();
});

test('should render Category list with error response', () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );

  render(
    <Router>
      <Category />
    </Router>
  );
  const categoryElement = screen.getByTestId('categorypage');
  expect(categoryElement).toBeDefined();
});

test('bloack a user  from user list when block button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.delete('https://localhost:8080/api/Category/status/2', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(blockCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const element = await screen.findByText(/Headphone/i);
  expect(element).toBeInTheDocument();

  const blkbtn = screen.getByTestId('blkbtn');
  expect(blkbtn).toBeInTheDocument();

  fireEvent.click(blkbtn);
});
test('Unblock a user  from user list when Unblock button is clicked get succeess response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.delete('https://localhost:8080/api/Category/status/1', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(unblockCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const element = await screen.findByText(/Mobile/i);
  expect(element).toBeInTheDocument();

  const unblkbtn = screen.getByTestId('unblkbtn');
  expect(unblkbtn).toBeInTheDocument();

  fireEvent.click(unblkbtn);
});
test('Unblock a user  from user list when Unblock button is clicked get error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.delete('https://localhost:8080/api/Category/status/1', (req, res, ctx) => {
      return res(ctx.status(400));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const element = await screen.findByText(/Mobile/i);
  expect(element).toBeInTheDocument();
  const unblkbtn = await screen.findByTestId('unblkbtn');
  expect(unblkbtn).toBeInTheDocument();
  fireEvent.click(unblkbtn);
});

test('Add category button with successs response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.post('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const addcategorybutton = await screen.findByTestId('addcategorybutton');
  expect(addcategorybutton).toBeInTheDocument();
  fireEvent.click(addcategorybutton);

  const addmodal = await screen.findByTestId('Add-input');
  expect(addmodal).toBeInTheDocument();
  const user = userEvent.setup();
  await user.type(screen.getByTestId('Add-input'), 'testcategory');
  expect(screen.getByTestId('Add-input')).toHaveValue('testcategory');

  const submitbutton = await screen.findByTestId('submitbutton');
  expect(submitbutton).toBeInTheDocument();
  fireEvent.click(submitbutton);
});
test('Add category button with white space not allowed', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.post('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const addcategorybutton = await screen.findByTestId('addcategorybutton');
  expect(addcategorybutton).toBeInTheDocument();
  fireEvent.click(addcategorybutton);

  const addmodal = await screen.findByTestId('Add-input');
  expect(addmodal).toBeInTheDocument();

  const user = userEvent.setup();

  await user.type(screen.getByTestId('Add-input'), '    ');

  const submitbutton = await screen.findByTestId('submitbutton');
  expect(submitbutton).toBeInTheDocument();
  fireEvent.click(submitbutton);
});
test('Add category button with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.post('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Category Already Exists' }));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const addcategorybutton = await screen.findByTestId('addcategorybutton');
  expect(addcategorybutton).toBeInTheDocument();
  fireEvent.click(addcategorybutton);

  const addmodal = await screen.findByTestId('Add-input');
  expect(addmodal).toBeInTheDocument();

  const user = userEvent.setup();

  await user.type(screen.getByTestId('Add-input'), 'testcategory');
  expect(screen.getByTestId('Add-input')).toHaveValue('testcategory');

  const submitbutton = await screen.findByTestId('submitbutton');
  expect(submitbutton).toBeInTheDocument();
  fireEvent.click(submitbutton);
});
test('edit category with success response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/Category/2', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const editcategorybutton = await screen.findByTestId('editcategorybutton2');
  expect(editcategorybutton).toBeInTheDocument();
  fireEvent.click(editcategorybutton);

  const editmodal = await screen.findByText('Edit Category');
  expect(editmodal).toBeInTheDocument();

  const user = userEvent.setup();
  expect(screen.getByTestId('category-input')).toHaveValue('Headphone');
  await user.type(screen.getByTestId('category-input'), 'S');

  expect(screen.getByTestId('category-input')).toHaveValue('HeadphoneS');
  const editsubmitbutton = await screen.findByTestId('editsubmitbutton');

  expect(editsubmitbutton).toBeInTheDocument();
  fireEvent.click(editsubmitbutton);
});
test('edit category with error response', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/Category/2', (req, res, ctx) => {
      return res(ctx.status(400), ctx.json({ message: 'Category Already Exists' }));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const editcategorybutton = await screen.findByTestId('editcategorybutton2');
  expect(editcategorybutton).toBeInTheDocument();
  fireEvent.click(editcategorybutton);

  const editmodal = await screen.findByText('Edit Category');
  expect(editmodal).toBeInTheDocument();

  const user = userEvent.setup();
  expect(screen.getByTestId('category-input')).toHaveValue('Headphone');
  await user.type(screen.getByTestId('category-input'), 'S');

  expect(screen.getByTestId('category-input')).toHaveValue('HeadphoneS');

  const editsubmitbutton = await screen.findByTestId('editsubmitbutton');
  expect(editsubmitbutton).toBeInTheDocument();
  fireEvent.click(editsubmitbutton);
});
test('edit category modal close on cancel click', async () => {
  server.use(
    rest.get('https://localhost:8080/api/Category', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  server.use(
    rest.put('https://localhost:8080/api/Category/2', (req, res, ctx) => {
      return res(ctx.status(200), ctx.json(getCategoryData));
    })
  );
  render(
    <Router>
      <Category />
    </Router>
  );
  const editcategorybutton = await screen.findByTestId('editcategorybutton2');
  expect(editcategorybutton).toBeInTheDocument();
  fireEvent.click(editcategorybutton);

  const editmodal = await screen.findByText('Edit Category');
  expect(editmodal).toBeInTheDocument();

  const canceleditmodal = await screen.findByTestId('canceleditmodal');
  expect(canceleditmodal).toBeInTheDocument();
});
