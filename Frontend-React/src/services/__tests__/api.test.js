import axios from 'axios'
import { createTodoItem, getTodoItems, updateTodoItem } from '../api'

jest.mock('axios')

describe('api should', () => {
  const env = process.env

  beforeEach(() => {
    jest.resetModules()
    process.env = { ...env }
  })

  afterEach(() => {
    jest.restoreAllMocks()
    process.env = env
  })

  it('call post with data when createTodoItem', async () => {
    const data = { id: '123', description: 'test' }
    axios.post.mockResolvedValue({ data })

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await createTodoItem(data)

    expect(axios.post).toHaveBeenCalledWith('https://test.api/api/TodoItems', data)
    expect(response).toEqual(data)
  })

  it('call get with no data when getTodoItems', async () => {
    const data = [
      { id: '123', description: 'test' },
      { id: '456', description: 'another test' },
    ]
    axios.get.mockResolvedValue({ data })

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await getTodoItems(data)

    expect(axios.get).toHaveBeenCalledWith('https://test.api/api/TodoItems')
    expect(response).toEqual(data)
  })

  it('call put with data when updateTodoItem', async () => {
    const data = { id: '123', description: 'test' }
    axios.put.mockResolvedValue({ data })

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await updateTodoItem(data)

    expect(axios.put).toHaveBeenCalledWith('https://test.api/api/TodoItems/123', data)
    expect(response).toEqual(data)
  })
})
