import { bgBlur, bgGradient, textGradient, filterStyles, hideScrollbarY, hideScrollbarX } from '../../utils/cssStyles';

describe('styles', () => {
  describe('bgBlur', () => {
    test('returns expected styles', () => {
      const props = {
        color: '#ff0000',
        blur: 10,
        opacity: 0.5,
        imgUrl: 'https://example.com/image.jpg',
      };
      const styles = bgBlur(props);
      expect(styles).toMatchSnapshot();
    });
  });

  describe('bgGradient', () => {
    test('returns expected styles', () => {
      const props = {
        direction: 'to top',
        startColor: '#ff0000',
        endColor: '#00ff00',
        imgUrl: 'https://example.com/image.jpg',
        color: '#0000ff',
      };
      const styles = bgGradient(props);
      expect(styles).toMatchSnapshot();
    });
  });

  describe('textGradient', () => {
    test('returns expected styles', () => {
      const value = 'red, blue';
      const styles = textGradient(value);
      expect(styles).toMatchSnapshot();
    });
  });

  describe('filterStyles', () => {
    test('returns expected styles', () => {
      const value = 'blur(10px) grayscale(50%)';
      const styles = filterStyles(value);
      expect(styles).toMatchSnapshot();
    });
  });

  describe('hideScrollbarY', () => {
    test('returns expected styles', () => {
      const styles = hideScrollbarY;
      expect(styles).toMatchSnapshot();
    });
  });

  describe('hideScrollbarX', () => {
    test('returns expected styles', () => {
      const styles = hideScrollbarX;
      expect(styles).toMatchSnapshot();
    });
  });
});
