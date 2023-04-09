import axios from 'axios'

export function createTodoItem(todoItemModel) {
  return axios.post(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`, todoItemModel)
}
