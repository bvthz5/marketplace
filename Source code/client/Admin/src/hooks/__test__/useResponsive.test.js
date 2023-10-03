import useMediaQuery from '@mui/material/useMediaQuery';
import useResponsive from '../useResponsive';
import { renderHook } from '@testing-library/react';

jest.mock('@mui/material/useMediaQuery'); // Mock the useMediaQuery hook

describe('useResponsive', () => {
  beforeEach(() => {
    jest.clearAllMocks(); // Clear the mock calls before each test
  });

  test('should return mediaUp when query is "up"', () => {
    const mediaUp = true; // Set the desired return value for mediaUp
    useMediaQuery.mockReturnValue(mediaUp);

    renderHook(() => useResponsive('up', 'sm', 'md'));
  });

  test('should return mediaDown when query is "down"', () => {
    const mediaDown = true; // Set the desired return value for mediaDown
    useMediaQuery.mockReturnValue(mediaDown);
    renderHook(() => useResponsive('down', 'sm', 'md'));
  });

  test('should return mediaBetween when query is "between"', () => {
    const mediaBetween = true; // Set the desired return value for mediaBetween
    useMediaQuery.mockReturnValue(mediaBetween);

    renderHook(() => useResponsive('between', 'sm', 'md'));
  });

  test('should return mediaOnly when query is not "up", "down", or "between"', () => {
    const mediaOnly = true; // Set the desired return value for mediaOnly
    useMediaQuery.mockReturnValue(mediaOnly);

    renderHook(() => useResponsive('other', 'sm', 'md'));
  });
});
