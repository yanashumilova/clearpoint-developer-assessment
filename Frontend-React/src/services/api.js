import axios from 'axios'

export function getTodoItems() {
  return axios.get(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`)
}

export function createTodoItem(todoItemModel) {
  return axios.post(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`, todoItemModel)
}
