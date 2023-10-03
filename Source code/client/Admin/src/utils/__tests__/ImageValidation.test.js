import { validateProfilePicture } from "../ImageValidation";
import { toast } from "react-toastify";

jest.mock("react-toastify", () => ({
  toast: {
    error: jest.fn(),
  },
}));

describe("validateProfilePicture", () => {
  it("should return true for valid file extensions", () => {
    const file = { name: "test.jpg" };
    const result = validateProfilePicture(file);
    expect(result).toBe(true);
  });

  it("should return false for invalid file extensions and show an error toast", () => {
    const file = { name: "test.txt" };
    const result = validateProfilePicture(file);
    expect(result).toBe(false);
    expect(toast.error).toHaveBeenCalledWith("Allowed Extensions are : *.jpeg, *.jpg, *.png, *.webp");
  });
});
