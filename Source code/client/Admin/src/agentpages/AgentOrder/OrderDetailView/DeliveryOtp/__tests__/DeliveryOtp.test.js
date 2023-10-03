import React from 'react';
const { render, screen, fireEvent, waitFor } = require('@testing-library/react');
const { default: DeliveryOtp } = require('../DeliveryOtp');
import { generateOtp, verifyOtp } from '../../../../../core/api/apiService';
import { toast } from 'react-toastify';
import { act } from 'react-test-renderer';
import useWindowDimensions from '../../../../../utils/WindowDimensions';

const toastError = jest.spyOn(toast, 'error').mockImplementation(() => {});
const toastSuccess = jest.spyOn(toast, 'success').mockImplementation(() => {});

jest.mock('../../../../../core/api/apiService', () => ({
  generateOtp: jest.fn(),
  verifyOtp: jest.fn(),
}));

jest.mock('../../../../../utils/WindowDimensions.js', () => ({
  __esModule: true,
  default: jest.fn().mockReturnValue({ width: 500, height: 600 }),
}));

describe('Modal Tests', () => {
  it('Deliver button is presents', () => {
    render(<DeliveryOtp />);

    const deliverButton = screen.getByTestId('deliver-button');
    expect(deliverButton).toBeInTheDocument();
  });

  it('Dialog window opens on button click', async () => {
    generateOtp.mockResolvedValue({});

    useWindowDimensions.mockReturnValue({ width: 300, height: 600 });

    render(<DeliveryOtp />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText('Generating otp...');
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();
  });

  it('Dialog window closes on button click', async () => {
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Wait for  seconds', data: 1 } } });

      render(<DeliveryOtp />);
    });

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const closeBtn = await screen.findByTestId('CloseIcon');
      expect(closeBtn).toBeInTheDocument();
      fireEvent.click(closeBtn);
    });

    const deliverButton2 = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton2);
  });
});

describe('Generate Otp', () => {
  it('Otp send', async () => {
    const email = 'test@gmail.com';

    generateOtp.mockResolvedValue({ data: { message: 'Success' } });

    render(<DeliveryOtp email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    expect(toastSuccess).toHaveBeenCalledWith('Otp Sent Successfully');
  });

  it('Otp timeout', async () => {
    const email = 'test@gmail.com';
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Wait for 10 seconds', data: 10 } } });
    });

    render(<DeliveryOtp email={email} />);

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const resendWaitMsg = await waitFor(() => screen.findByTestId('wait-for-msg'), {
      timeout: 3000,
    });
    expect(resendWaitMsg).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();
  });

  it('Otp resend', async () => {
    const email = 'test@gmail.com';
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Wait for 0 seconds', data: 0 } } });
    });

    render(<DeliveryOtp email={email} />);

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const resendBtn = await screen.findByText('Resend');
      generateOtp.mockResolvedValue({ data: { message: 'success' } });

      expect(resendBtn).toBeInTheDocument();
      expect(resendBtn).toBeEnabled();

      fireEvent.click(resendBtn);
    });

    expect(toastSuccess).toHaveBeenCalledWith('Otp Sent Successfully');

    const resendWaitMsg = await waitFor(() => screen.findByTestId('wait-for-msg'), {
      timeout: 3000,
    });
    expect(resendWaitMsg).toBeInTheDocument();
  });

  it('Already Delivered', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Order Status : DELIVERED' } } });
    });

    render(<DeliveryOtp email={email} onCompleted={onCompleted} />);

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    expect(onCompleted).toBeCalled();

    const resendWaitMsg = await screen.findByText('Order Already Marked As Delivered');
    expect(resendWaitMsg).toBeInTheDocument();
  });

  it('Unexpected Error', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Server Error' } } });
    });

    render(<DeliveryOtp email={email} onCompleted={onCompleted} />);

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });
  });
});

describe('Verify Otp', () => {
  it('Otp type, Invalid Otp', async () => {
    const email = 'test@gmail.com';

    await act(() => {
      verifyOtp.mockRejectedValue({ response: { data: { message: 'Invalid Otp' } } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });

    expect(toastError).toHaveBeenCalledWith('Invalid Otp', { toastId: 'Invalid Otp' });
  });

  it('Otp type, Already Delivered', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockRejectedValue({ response: { data: { message: 'Order Status : DELIVERED' } } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });

    const resendWaitMsg = await screen.findByText('Order Already Marked As Delivered');
    expect(resendWaitMsg).toBeInTheDocument();
  });

  it('Otp type, Unexpected Error', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockRejectedValue({ response: { data: { message: 'Server Error' } } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });
  });

  it('Otp type, Otp Timeout', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockRejectedValue({ response: { data: { message: 'Otp Timeout' } } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });

    expect(toastError).toHaveBeenCalledWith('Otp Timeout');
  });

  it('Otp type Success, Marked as Delivered', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockResolvedValue({ data: { message: 'Marked As Delivered' } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });
  });

  it('Otp type Key Down', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockResolvedValue({ data: { message: 'Marked As Delivered' } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });

      fireEvent.keyDown(otpFields[5], { key: 'Backspace', code: 'Backspace' });
      fireEvent.keyDown(otpFields[5], { key: 'Enter', code: 'Enter' });

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeDisabled();

      fireEvent.change(otpFields[5], { target: { value: 1 } });

      const verifyBtn3 = await screen.findByText('Verify');
      expect(verifyBtn3).toBeInTheDocument();
      expect(verifyBtn3).toBeEnabled();

      fireEvent.click(verifyBtn2);
    });
  });

  it('Otp type Non numeric', async () => {
    const email = 'test@gmail.com';
    const onCompleted = jest.fn();

    await act(() => {
      verifyOtp.mockResolvedValue({ data: { message: 'Marked As Delivered' } });
      generateOtp.mockResolvedValue({ data: { message: 'Success' } });
    });

    render(<DeliveryOtp onCompleted={onCompleted} email={email} />);

    const deliverButton = screen.getByTestId('deliver-button');
    fireEvent.click(deliverButton);

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const otpGeneratingMessage = await screen.findByText(`Please enther the otp sent to ${email}`);
    expect(otpGeneratingMessage).toBeInTheDocument();

    const verifyBtn = await screen.findByText('Verify');
    expect(verifyBtn).toBeInTheDocument();
    expect(verifyBtn).toBeDisabled();

    await act(async () => {
      const otpFields = await screen.findAllByPlaceholderText('-');

      fireEvent.change(otpFields[0], { target: { value: 1 } });
      fireEvent.change(otpFields[1], { target: { value: 1 } });
      fireEvent.change(otpFields[2], { target: { value: 1 } });
      fireEvent.change(otpFields[3], { target: { value: 1 } });
      fireEvent.change(otpFields[4], { target: { value: 1 } });
      fireEvent.change(otpFields[5], { target: { value: 'a' } });
      fireEvent.change(otpFields[4], { target: { value: '' } });
      otpFields[5].dispatchEvent(new Event('change'));

      const verifyBtn2 = await screen.findByText('Verify');
      expect(verifyBtn2).toBeInTheDocument();
      expect(verifyBtn2).toBeDisabled();

      fireEvent.click(verifyBtn2);
    });
  });
});

describe('Timer Test', () => {
  it('Timer Works', async () => {
    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Wait for 2 seconds', data: 2 } } });
    });

    await act(() => {
      render(<DeliveryOtp setInterval={jest.fn()} />);
    });

    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    const dialogTitle = screen.getByTestId('dialog-title');
    expect(dialogTitle).toBeInTheDocument();

    const resendWaitMsg = await waitFor(async () => await screen.findByText(/wait for 1/i), {
      timeout: 2000,
    });

    expect(resendWaitMsg).toBeInTheDocument();

    await new Promise((resolve) => setTimeout(resolve, 2000));

    expect(await screen.findByText(/resend/i)).toBeEnabled();

    await act(() => {
      generateOtp.mockRejectedValue({ response: { data: { message: 'Wait for 0 seconds', data: 0 } } });
    });
    await act(() => {
      const deliverButton = screen.getByTestId('deliver-button');
      fireEvent.click(deliverButton);
    });

    await new Promise((resolve) => setTimeout(resolve, 1000));

    expect(await screen.findByText(/resend/i)).toBeEnabled();
  });
});
