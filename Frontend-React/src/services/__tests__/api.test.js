import axios from 'axios'
import { createTodoItem } from '../api'

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
    axios.post.mockResolvedValue(data)

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await createTodoItem(data)

    expect(axios.post).toHaveBeenCalledWith('https://test.api/api/TodoItems', data)
    expect(response).toEqual(data)
  })
})
