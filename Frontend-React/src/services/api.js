import axios from 'axios'

export function getTodoItems() {
  return axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`).then((response) => response.data)
}

export function updateTodoItem(todoItemModel) {
  return axios
    .put(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems/${todoItemModel.id}`, todoItemModel)
    .then((response) => response.data)
}

export function createTodoItem(todoItemModel) {
  return axios
    .post(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`, todoItemModel)
    .then((response) => response.data)
}
