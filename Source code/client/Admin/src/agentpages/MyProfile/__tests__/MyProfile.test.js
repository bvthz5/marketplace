import '@testing-library/jest-dom/extend-expect';
import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter as Router } from 'react-router-dom';
import { MyProfile } from '../MyProfile';
import userEvent from '@testing-library/user-event';
import { getAgentProfile, editAgentProfile, updateProfilePic } from '../../../core/api/apiService';
import { act } from 'react-dom/test-utils';
import { validateProfilePicture } from '../../../utils/ImageValidation';
import { Provider } from 'react-redux';
import { store } from '../../../redux/store';

jest.mock('../../../core/api/apiService', () => {
  return {
    getAgentProfile: jest.fn(),
    editAgentProfile: jest.fn(),
    updateProfilePic: jest.fn(),
  };
});

jest.mock('../../../utils/ImageValidation', () => {
  return {
    validateProfilePicture: jest.fn(() => true),
  };
});

describe('Basic Tests', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  getAgentProfile.mockResolvedValue({
    data: { data: { name: 'Sneh', phoneNumber: '1234567890', email: 'email@gmail.com' } },
  });

  it('Component render test', () => {
    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );
  });

  it('Name field present', async () => {
    const user = userEvent.setup();
    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const field = screen.getByTestId('name-input').querySelector('input');
    expect(field).toBeInTheDocument();

    await user.type(field, 's');

    // fireEvent.change(field , {target: { value: 'some text'}});
    expect(field.value).toBe('Snehs');

    const cancelBtn = screen.findByTestId('cancel-btn');
    expect(cancelBtn).toBeDefined();
  });

  it('Name field error', async () => {
    const user = userEvent.setup();

    getAgentProfile.mockRejectedValue({ data: { message: 'Failed for Name Field' } });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const field = screen.getByTestId('name-input').querySelector('input');
    expect(field).toBeInTheDocument();

    await user.type(field, 's');

    // fireEvent.change(field , {target: { value: 'some text'}});
    expect(field.value).toBe('s');

    const cancelBtn = screen.findByTestId('cancel-btn');
    expect(cancelBtn).toBeDefined();
  });

  it('Cancelled btn', async () => {
    const user = userEvent.setup();

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const field = screen.getByTestId('name-input').querySelector('input');
    expect(field).toBeInTheDocument();

    await user.type(field, 's');

    expect(field.value).toBe('s');

    const cancelBtn = screen.getByTestId('cancel-btn');
    expect(cancelBtn).toBeDefined();

    fireEvent.click(cancelBtn);

    expect(field.value).toBe('');
  });
});

// Form Submission

describe('Form Submission', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('Submit form with valid data', async () => {
    getAgentProfile.mockResolvedValue({
      data: {
        data: { name: 'Agent Old Name', phoneNumber: '1234567890', email: 'agent@gmail.com', profilePic: 'profilepic' },
      },
    });

    editAgentProfile.mockResolvedValue({
      data: { data: { name: 'Agent New Name', phoneNumber: '0123456789', email: 'agent@gmail.com' } },
    });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const nameInputField = screen.getByTestId('name-input').querySelector('input');
    expect(nameInputField).toBeInTheDocument();

    fireEvent.change(nameInputField, { target: { value: 'Agent New Name' } });

    expect(nameInputField.value).toBe('Agent New Name');

    const phoneNumberInputField = screen.getByTestId('phoneNumber-input').querySelector('input');

    expect(phoneNumberInputField).toBeInTheDocument();

    fireEvent.change(phoneNumberInputField, { target: { value: '0123456789' } });

    expect(phoneNumberInputField.value).toBe('0123456789');

    const updateBtn = screen.getByTestId('update-btn');
    expect(updateBtn).toBeDefined();

    fireEvent.click(updateBtn);
  });

  it('Submit form with valid data but api fails', async () => {
    getAgentProfile.mockResolvedValue({
      data: { data: { name: 'Agent Old Name', phoneNumber: '1234567890', email: 'agent@gmail.com' } },
    });

    editAgentProfile.mockRejectedValue({
      data: { message: 'Api Failed' },
    });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const nameInputField = screen.getByTestId('name-input').querySelector('input');
    expect(nameInputField).toBeInTheDocument();

    fireEvent.change(nameInputField, { target: { value: 'Agent New Name' } });

    expect(nameInputField.value).toBe('Agent New Name');

    const phoneNumberInputField = screen.getByTestId('phoneNumber-input').querySelector('input');

    expect(phoneNumberInputField).toBeInTheDocument();

    fireEvent.change(phoneNumberInputField, { target: { value: '0123456789' } });

    expect(phoneNumberInputField.value).toBe('0123456789');

    const updateBtn = screen.getByTestId('update-btn');
    expect(updateBtn).toBeDefined();

    fireEvent.click(updateBtn);
  });

  it('Submit form with no change', async () => {
    getAgentProfile.mockResolvedValue({
      data: { data: { name: 'Agent Old Name', phoneNumber: '1234567890', email: 'agent@gmail.com' } },
    });

    editAgentProfile.mockRejectedValue({
      data: { message: 'Api Failed' },
    });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );
    act(() => {
      const nameInputField = screen.getByTestId('name-input').querySelector('input');
      expect(nameInputField).toBeInTheDocument();

      fireEvent.change(nameInputField, { target: { value: 'Agent Old Name' } });

      const phoneNumberInputField = screen.getByTestId('phoneNumber-input').querySelector('input');

      expect(phoneNumberInputField).toBeInTheDocument();

      fireEvent.change(phoneNumberInputField, { target: { value: '1234567890' } });

      expect(phoneNumberInputField.value).toBe('1234567890');

      const updateBtn = screen.getByTestId('update-btn');
      expect(updateBtn).toBeDefined();

      fireEvent.click(updateBtn);
    });
  });
});

describe('Profile Pic Upload', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('Image validation failed', async () => {
    validateProfilePicture.mockImplementationOnce(() => false);

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );
    const input = screen.getByTestId('image-uploader');
    await waitFor(() => {
      expect(input.files.length).toBe(0);
    });
  });

  it('Image Upload Success, Get Agent Profile Failed', async () => {
    updateProfilePic.mockResolvedValue({ data: { message: 'Success for Image Upload' } });
    getAgentProfile.mockRejectedValue({ data: { message: 'Failed for Image Upload' } }).mockResolvedValueOnce({
      data: {
        data: { name: 'Agent Old Name', phoneNumber: '1234567890', email: 'agent@gmail.com', profilePic: 'profilepic' },
      },
    });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const input = screen.getByTestId('image-uploader');
    fireEvent.change(input, {
      target: {
        files: [new File(['(⌐□_□)'], 'chucknorris.png', { type: 'image/png' })],
      },
    });
  });

  it('Image Upload Fail', async () => {
    updateProfilePic.mockRejectedValue({ data: { message: 'Failed for Imgage Upload Fail' } });
    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const input = screen.getByTestId('image-uploader');
    fireEvent.change(input, {
      target: {
        files: [new File(['(⌐□_□)'], 'chucknorris.png', { type: 'image/png' })],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });
  });

  it('Image Upload Success', async () => {
    updateProfilePic.mockResolvedValue({ data: { message: 'Success' } });
    getAgentProfile.mockResolvedValue({
      data: {
        data: { name: 'Agent Old Name', phoneNumber: '1234567890', email: 'agent@gmail.com', profilePic: 'profilepic' },
      },
    });

    render(
      <Provider store={store}>
        <Router>
          <MyProfile />
        </Router>
      </Provider>
    );

    const input = screen.getByTestId('image-uploader');
    fireEvent.change(input, {
      target: {
        files: [new File(['(⌐□_□)'], 'chucknorris.png', { type: 'image/png' })],
      },
    });
    await waitFor(() => {
      expect(input.files.length).toBe(1);
    });
  });
});
