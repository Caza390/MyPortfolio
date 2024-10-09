import axios from 'axios';

const API_URL = 'http://localhost:5176/api/ToDos'; // Replace {port} with your backend port

class ToDoService {
  getToDos() {
    return axios.get(API_URL);
  }

  getToDo(id) {
    return axios.get(`${API_URL}/${id}`);
  }

  createToDo(data) {
    return axios.post(API_URL, data);
  }

  updateToDo(id, data) {
    return axios.put(`${API_URL}/${id}`, data);
  }

  deleteToDo(id) {
    return axios.delete(`${API_URL}/${id}`);
  }
}

export default new ToDoService();
