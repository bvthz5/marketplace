import { convertTime } from "../Utils";


describe('convertTime', () => {
    test('converts time correctly for AM', () => {
      // Arrange
      const time = '2023-05-22T09:30:00';
  
      // Act
      const convertedTime = convertTime(time);
  
      // Assert
      expect(convertedTime).toBe('9:30 AM');
    });
  
    test('converts time correctly for PM', () => {
      // Arrange
      const time = '2023-05-22T18:45:00';
  
      // Act
      const convertedTime = convertTime(time);
  
      // Assert
      expect(convertedTime).toBe('6:45 PM');
    });
  
    test('converts 12:00 AM correctly', () => {
      // Arrange
      const time = '2023-05-22T00:00:00';
  
      // Act
      const convertedTime = convertTime(time);
  
      // Assert
      expect(convertedTime).toBe('12:00 AM');
    });
  
    test('converts 12:00 PM correctly', () => {
      // Arrange
      const time = '2023-05-22T12:00:00';
  
      // Act
      const convertedTime = convertTime(time);
  
      // Assert
      expect(convertedTime).toBe('12:00 PM');
    });
  
    test('converts single-digit minutes correctly', () => {
      // Arrange
      const time = '2023-05-22T10:05:00';
  
      // Act
      const convertedTime = convertTime(time);
  
      // Assert
      expect(convertedTime).toBe('10:05 AM');
    });
  });