import { toast } from "react-toastify";
import { validateProductImages } from "../Utils";


describe('validateProductImages', () => {
  it('returns true for valid file extensions', () => {
    const image = { name: 'example.jpeg' };
    expect(validateProductImages(image)).toBe(true);
  });

  it('returns false for invalid file extensions', () => {
    const image = { name: 'example.pdf' };
    expect(validateProductImages(image)).toBe(false);
  });

  it('displays an error message for invalid file extensions', () => {
    const toastSpy = jest.spyOn(toast, 'error').mockImplementation(() => {});
    const image = { name: 'example.pdf' };
    expect(validateProductImages(image)).toBe(false);
    expect(toastSpy).toHaveBeenCalledWith(
      'Allowed Extensions are : *.jpeg, *.jpg, *.png, *.webp'
    );
    toastSpy.mockRestore();
  });
});