import axios from 'axios';


const URL = process.env.REACT_APP_API_PATH
  ? process.env.REACT_APP_API_PATH
  : "https://localhost:8080";

export default axios.create({
  baseURL: `${URL}`,
  headers: {
    "Content-type": "application/json",
  },
});
