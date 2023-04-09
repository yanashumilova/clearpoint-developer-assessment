import axios from 'axios'

export class ApiError extends Error {
  constructor(message) {
    super(message)
    this.name = 'ApiError'
  }
}

export function getTodoItems() {
  return axios
    .get(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`)
    .then((response) => response.data)
    .catch(() => {
      throw new ApiError('Something went wrong. Please refresh the list.')
    })
}

export function updateTodoItem(todoItemModel) {
  return axios
    .put(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems/${todoItemModel.id}`, todoItemModel)
    .then((response) => response.data)
    .catch((error) => {
      switch (error.response.status) {
        case 400:
          throw new ApiError(error.response.data)
        case 404:
          throw new ApiError('The item is not found. Please refresh the list.')
        default:
          throw new ApiError('Something went wrong. Please try again.')
      }
    })
}

export function createTodoItem(todoItemModel) {
  return axios
    .post(`${process.env.REACT_APP_API_BASE_URL}/api/TodoItems`, todoItemModel)
    .then((response) => response.data)
    .catch((error) => {
      switch (error.response.status) {
        case 400:
          throw new ApiError(error.response.data)
        default:
          throw new ApiError('Something went wrong. Please try again.')
      }
    })
}
