<template>
  <div>
    <h1>ToDo List</h1>
    <ul>
      <li v-for="todo in todos" :key="todo.ID">
        <strong>{{ todo.Name }}</strong>: {{ todo.Description }}
        <button @click="deleteToDo(todo.ID)">Delete</button>
      </li>
    </ul>
    <h2>Add New ToDo</h2>
    <form @submit.prevent="addToDo">
      <div>
        <label for="name">Name:</label>
        <input type="text" v-model="newToDo.Name" required />
      </div>
      <div>
        <label for="description">Description:</label>
        <input type="text" v-model="newToDo.Description" />
      </div>
      <button type="submit">Add</button>
    </form>
  </div>
</template>

<script>
import ToDoService from '../services/ToDoService';

export default {
  data() {
    return {
      todos: [],
      newToDo: {
        Name: '',
        Description: ''
      }
    };
  },
  methods: {
    fetchToDos() {
      ToDoService.getToDos()
        .then(response => {
          this.todos = response.data;
        })
        .catch(error => {
          console.error('Error fetching ToDos:', error);
        });
    },
    addToDo() {
      ToDoService.createToDo(this.newToDo)
        .then(() => {
          this.fetchToDos();
          this.newToDo.Name = '';
          this.newToDo.Description = '';
        })
        .catch(error => {
          console.error('Error adding ToDo:', error);
        });
    },
    deleteToDo(id) {
      ToDoService.deleteToDo(id)
        .then(() => {
          this.fetchToDos();
        })
        .catch(error => {
          console.error('Error deleting ToDo:', error);
        });
    }
  },
  mounted() {
    this.fetchToDos();
  }
};
</script>

<style scoped>
/* Add your styles here */
</style>
