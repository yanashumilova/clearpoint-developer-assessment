import axios from 'axios'
import { ApiError, createTodoItem, getTodoItems, updateTodoItem } from '../api'

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

  it('throw ApiError when createTodoItem given request fails', async () => {
    const data = { id: '123', description: 'test' }
    axios.post.mockRejectedValue({ response: { status: 500 } })

    const act = () => createTodoItem(data)

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('Something went wrong. Please try again.')
  })

  it('throw ApiError with API message when createTodoItem given request fails with 400', async () => {
    const data = { id: '123', description: 'test' }
    axios.post.mockRejectedValue({ response: { status: 400, data: 'Duplicate description' } })

    const act = () => createTodoItem(data)

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('Duplicate description')
  })

  it('call get with no data when getTodoItems', async () => {
    const data = [
      { id: '123', description: 'test' },
      { id: '456', description: 'another test' },
    ]
    axios.get.mockResolvedValue({ data })

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await getTodoItems()

    expect(axios.get).toHaveBeenCalledWith('https://test.api/api/TodoItems')
    expect(response).toEqual(data)
  })

  it('throw ApiError when getTodoItems given request fails', async () => {
    axios.get.mockRejectedValue({ response: { status: 500 } })

    const act = () => getTodoItems()

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('Something went wrong. Please refresh the list.')
  })

  it('call put with data when updateTodoItem', async () => {
    const data = { id: '123', description: 'test' }
    axios.put.mockResolvedValue({ data })

    process.env.REACT_APP_API_BASE_URL = 'https://test.api'
    const response = await updateTodoItem(data)

    expect(axios.put).toHaveBeenCalledWith('https://test.api/api/TodoItems/123', data)
    expect(response).toEqual(data)
  })

  it('throw ApiError when updateTodoItem given request fails', async () => {
    const data = { id: '123', description: 'test' }
    axios.put.mockRejectedValue({ response: { status: 500 } })

    const act = () => updateTodoItem(data)

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('Something went wrong. Please try again.')
  })

  it('throw ApiError when updateTodoItem given request fails with 404', async () => {
    const data = { id: '123', description: 'test' }
    axios.put.mockRejectedValue({ response: { status: 404 } })

    const act = () => updateTodoItem(data)

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('The item is not found. Please refresh the list.')
  })

  it('throw ApiError and API message when updateTodoItem given request fails with 400', async () => {
    const data = { id: '123', description: 'test' }
    axios.put.mockRejectedValue({ response: { status: 400, data: 'Duplicate description' } })

    const act = () => updateTodoItem(data)

    await expect(act).rejects.toThrow(ApiError)
    await expect(act).rejects.toThrow('Duplicate description')
  })
})
