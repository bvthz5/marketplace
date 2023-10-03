import '@testing-library/jest-dom/extend-expect';
import { fireEvent, render, screen } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import React from 'react';
import DeliveryAgent from '../DeliveryAgent';
import { rest, server } from '../../../../src/testServer';
import userEvent from '@testing-library/user-event';
import { deleteAgentResponse, getAgentData, getSingleAgentData, getSingleAgentDataBlock } from '../testData/data';
import Swal from 'sweetalert2';

server.use(
  rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
    return res(ctx.status(200), ctx.json(getAgentData));
  })
);

jest.mock('sweetalert2', () => ({
  fire: jest.fn().mockResolvedValue({ isConfirmed: true }),
}));

describe('agentlist', () => {
  test('should render AgentDashboard component', () => {
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const DeliveryAgentElement = screen.getByTestId('deliveryagentpage');
    expect(DeliveryAgentElement).toBeDefined();
  });

  test('remove a agent  from agent list when delete button is clicked get succeess response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleAgentData));
      })
    );

    server.use(
      rest.put('https://localhost:8080/api/agent/status/6', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(deleteAgentResponse));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();

    const deletebtn = screen.getByTestId('deletebtn');
    expect(deletebtn).toBeInTheDocument();

    fireEvent.click(deletebtn);
    await expect(Swal.fire).toBeCalled();
  });

  test('remove a agent  from agent list when delete button is clicked get error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleAgentData));
      })
    );

    server.use(
      rest.put('https://localhost:8080/api/agent/status/6', (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();

    const deletebtn = screen.getByTestId('deletebtn');
    expect(deletebtn).toBeInTheDocument();

    fireEvent.click(deletebtn);
  });
  test('block a agent  from agent list when block button is clicked get succeess response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleAgentData));
      })
    );
    server.use(
      rest.put('https://localhost:8080/api/agent/status/6', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(deleteAgentResponse));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();
    const blockbtn = screen.getByTestId('blockbtn');

    expect(blockbtn).toBeInTheDocument();
    fireEvent.click(blockbtn);

    await expect(Swal.fire).toBeCalled();
  });

  test('block a agent  from agent list when block button is clicked get error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleAgentData));
      })
    );
    server.use(
      rest.put('https://localhost:8080/api/agent/status/6', (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({ message: 'Agent Already Exist' }));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();
    const blockbtn = screen.getByTestId('blockbtn');

    expect(blockbtn).toBeInTheDocument();
    fireEvent.click(blockbtn);
  });

  test('unblock a agent  from agent list when unblock button is clicked get succeess response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getSingleAgentDataBlock));
      })
    );
    server.use(
      rest.put('https://localhost:8080/api/agent/status/6', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(deleteAgentResponse));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();
    const unblockbtn = screen.getByTestId('unblockbtn');

    expect(unblockbtn).toBeInTheDocument();
    fireEvent.click(unblockbtn);

    await expect(Swal.fire).toBeCalled();
  });

  test('Add agent is working with success response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    server.use(
      rest.post('https://localhost:8080/api/agent/6', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const addagentbtn = screen.getByTestId('addagentbtn');
    expect(addagentbtn).toBeInTheDocument();
    fireEvent.click(addagentbtn);

    const addagentmodal = screen.getByTestId('addagentmodal');
    expect(addagentmodal).toBeInTheDocument();

    const user = userEvent.setup();
    await user.type(screen.getByTestId('name-input'), 'testagent');
    expect(screen.getByTestId('name-input')).toHaveValue('testagent');

    await user.type(screen.getByTestId('email-input'), 'testagent@gmail.com');
    expect(screen.getByTestId('email-input')).toHaveValue('testagent@gmail.com');

    await user.type(screen.getByTestId('number-input'), '9090909090');
    expect(screen.getByTestId('number-input')).toHaveValue('9090909090');

    const submit = screen.getByTestId('submit');
    expect(submit).toBeInTheDocument();
    fireEvent.click(submit);
  });

  test('Add agent is working with error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    server.use(
      rest.post('https://localhost:8080/api/agent/6', (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({message:"Agent Already Exist"}));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const addagentbtn = screen.getByTestId('addagentbtn');
    expect(addagentbtn).toBeInTheDocument();
    fireEvent.click(addagentbtn);

    const addagentmodal = screen.getByTestId('addagentmodal');
    expect(addagentmodal).toBeInTheDocument();

    const user = userEvent.setup();
    await user.type(screen.getByTestId('name-input'), 'testagent');
    expect(screen.getByTestId('name-input')).toHaveValue('testagent');

    await user.type(screen.getByTestId('email-input'), 'testagent@gmail.com');
    expect(screen.getByTestId('email-input')).toHaveValue('testagent@gmail.com');

    await user.type(screen.getByTestId('number-input'), '9090909090');
    expect(screen.getByTestId('number-input')).toHaveValue('9090909090');

    const submit = screen.getByTestId('submit');
    expect(submit).toBeInTheDocument();
    fireEvent.click(submit);
  });

  test('Add agent is working with unhandled error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    server.use(
      rest.post('https://localhost:8080/api/agent/6', (req, res, ctx) => {
        return res(ctx.status(400), ctx.json({message:"unhandled error response"}));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const addagentbtn = screen.getByTestId('addagentbtn');
    expect(addagentbtn).toBeInTheDocument();
    fireEvent.click(addagentbtn);

    const addagentmodal = screen.getByTestId('addagentmodal');
    expect(addagentmodal).toBeInTheDocument();

    const user = userEvent.setup();
    await user.type(screen.getByTestId('name-input'), 'testagent');
    expect(screen.getByTestId('name-input')).toHaveValue('testagent');

    await user.type(screen.getByTestId('email-input'), 'testagent@gmail.com');
    expect(screen.getByTestId('email-input')).toHaveValue('testagent@gmail.com');

    await user.type(screen.getByTestId('number-input'), '9090909090');
    expect(screen.getByTestId('number-input')).toHaveValue('9090909090');

    const submit = screen.getByTestId('submit');
    expect(submit).toBeInTheDocument();
    fireEvent.click(submit);
  });
  test('Edit  agent is working with success response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    server.use(
      rest.put('https://localhost:8080/api/agent/6', (req, res, ctx) => {
        return res(ctx.status(200));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();

    const editagent = await screen.findByTestId('editagentbutton');
    expect(editagent).toBeInTheDocument();
    fireEvent.click(editagent);

    const editagentmodal = await screen.findByTestId('editagentmodal');
    expect(editagentmodal).toBeInTheDocument();

    const user = userEvent.setup();
    expect(screen.getByTestId('name-input')).toHaveValue('Stejin');
    await user.type(screen.getByTestId('name-input'), 'S');

    expect(screen.getByTestId('name-input')).toHaveValue('StejinS');

    expect(screen.getByTestId(/number-input/i)).toHaveValue('8585858548');

    userEvent.clear(screen.getByTestId(/number-input/i));
    await user.type(screen.getByTestId(/number-input/i), '8888888888');
    expect(screen.getByTestId(/number-input/i)).toHaveValue('8888888888');

    const submitbuttonedit = await screen.findByTestId('submitbuttonedit');
    expect(submitbuttonedit).toBeInTheDocument();
    fireEvent.click(submitbuttonedit);
  });
  test('Edit  agent is working with error response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
    server.use(
      rest.put('https://localhost:8080/api/agent/6', (req, res, ctx) => {
        return res(ctx.status(400));
      })
    );
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
    const element = await screen.findByText(/stejin.jacob@innovaturelabs.com/i);
    expect(element).toBeInTheDocument();

    const editagent = await screen.findByTestId('editagentbutton');
    expect(editagent).toBeInTheDocument();
    fireEvent.click(editagent);

    const editagentmodal = await screen.findByTestId('editagentmodal');
    expect(editagentmodal).toBeInTheDocument();

    const user = userEvent.setup();
    expect(screen.getByTestId('name-input')).toHaveValue('Stejin');
    await user.type(screen.getByTestId('name-input'), 'S');

    expect(screen.getByTestId('name-input')).toHaveValue('StejinS');

    expect(screen.getByTestId(/number-input/i)).toHaveValue('8585858548');

    userEvent.clear(screen.getByTestId(/number-input/i));
    await user.type(screen.getByTestId(/number-input/i), '8888888888');
    expect(screen.getByTestId(/number-input/i)).toHaveValue('8888888888');

    const submitbuttonedit = await screen.findByTestId('submitbuttonedit');
    expect(submitbuttonedit).toBeInTheDocument();
    fireEvent.click(submitbuttonedit);
  });


  test('Search agent with success response', async () => {
    server.use(
      rest.get('https://localhost:8080/api/agent', (req, res, ctx) => {
        return res(ctx.status(200), ctx.json(getAgentData));
      })
    );
  
    render(
      <Router>
        <DeliveryAgent />
      </Router>
    );
   const searchbar=await screen.findByTestId('searchbar');
   expect (searchbar).toBeInTheDocument();
   const searchinput = await screen.findByTestId('Search-input');
   expect(searchinput).toBeInTheDocument();

   const user = userEvent.setup();
   await user.type(screen.getByTestId(/Search-input/i), 'Ste');
   
  });
});