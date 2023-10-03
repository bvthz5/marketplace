import { toast } from "react-toastify";

export const validateProfilePicture = (profilePic) => {
    let allowedExtension = ['jpeg', 'jpg', 'png', 'webp'];
    let fileExtension = profilePic.name.split('.').pop().toLowerCase();
    let isValidFile = false;

    for (let index in allowedExtension) {
      if (fileExtension === allowedExtension[index]) {
        isValidFile = true;
        break;
      }
    }
    if (!isValidFile) {
      toast.error('Allowed Extensions are : *.' + allowedExtension.join(', *.'));
    }
    return isValidFile;
  };