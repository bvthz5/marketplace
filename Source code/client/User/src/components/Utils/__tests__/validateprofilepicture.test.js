import { toast } from "react-toastify";
import { validateProfilePicture } from "../Utils";

describe("validateProfilePicture", () => {
  it("returns true for valid file extensions", () => {
    const profilePic = [{ name: "example.jpeg" }];
    expect(validateProfilePicture(profilePic)).toBe(true);
  });

  it("returns false for invalid file extensions", () => {
    const profilePic = [{ name: "example.pdf" }];
    expect(validateProfilePicture(profilePic)).toBe(false);
  });

  it("displays an error message for invalid file extensions", () => {
    const toastSpy = jest.spyOn(toast, "error").mockImplementation(() => {});
    const profilePic = [{ name: "example.pdf" }];
    expect(validateProfilePicture(profilePic)).toBe(false);
    expect(toastSpy).toHaveBeenCalledWith(
      "Allowed Extensions are : *.jpeg, *.jpg, *.png, *.webp"
    );
    toastSpy.mockRestore();
  });
});
