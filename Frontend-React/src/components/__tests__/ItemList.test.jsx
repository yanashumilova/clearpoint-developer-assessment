import { render, screen, waitFor, within } from '@testing-library/react'
import ItemList from '../ItemList'
import userEvent from '@testing-library/user-event'

const mockGetTodoItems = jest.fn()
const mockUpdateTodoItem = jest.fn()

jest.mock('../../services/api.js', () => ({
  getTodoItems: (...args) => mockGetTodoItems(...args),
  updateTodoItem: (...args) => mockUpdateTodoItem(...args),
}))

describe('ItemList should', () => {
  it('load items on mount', async () => {
    mockGetTodoItems.mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])

    render(<ItemList />)

    expect(mockGetTodoItems).toHaveBeenCalledTimes(1)
    expect(mockGetTodoItems).toHaveBeenCalledWith()

    await waitFor(() => {
      expect(screen.getByText(/3 item/i)).toBeInTheDocument()
    })
  })

  it('show error when load items on mount given error thrown', async () => {
    mockGetTodoItems.mockRejectedValueOnce(new Error('something went wrong'))

    render(<ItemList />)

    expect(await screen.findByRole('alert')).toHaveTextContent('something went wrong')
  })

  it('load items on refresh', async () => {
    mockGetTodoItems
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }, { id: '000' }])

    render(<ItemList />)

    await screen.findByText(/3 item/i)

    await userEvent.click(screen.getByRole('button', { name: /refresh/i }))

    expect(mockGetTodoItems).toHaveBeenCalledTimes(2)
    expect(mockGetTodoItems).toHaveBeenLastCalledWith()

    await waitFor(() => {
      expect(screen.getByText(/4 item/i)).toBeInTheDocument()
    })
  })

  it('show error when load items on refresh given error thrown', async () => {
    mockGetTodoItems
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])
      .mockRejectedValueOnce(new Error('something went wrong'))

    render(<ItemList />)
    await screen.findByText(/3 item/i)
    await userEvent.click(screen.getByRole('button', { name: /refresh/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent('something went wrong')
  })

  it('mark complete and reload items on mark as complete', async () => {
    mockGetTodoItems
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456', description: 'test' }, { id: '789' }])
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '789' }])
    mockUpdateTodoItem.mockResolvedValueOnce({})

    render(<ItemList />)

    const item = await screen.findByTestId('item-456')

    await userEvent.click(within(item).getByRole('button'))

    expect(mockUpdateTodoItem).toHaveBeenCalledTimes(1)
    expect(mockUpdateTodoItem).toHaveBeenCalledWith({
      id: '456',
      description: 'test',
      isCompleted: true,
    })

    expect(mockGetTodoItems).toHaveBeenCalledTimes(2)
    expect(mockGetTodoItems).toHaveBeenLastCalledWith()

    await waitFor(() => {
      expect(screen.getByText(/2 item/i)).toBeInTheDocument()
    })
  })

  it('show error when mark complete given error thrown', async () => {
    mockGetTodoItems
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])
    mockUpdateTodoItem.mockRejectedValueOnce(new Error('something went wrong'))

    render(<ItemList />)
    const item = await screen.findByTestId('item-456')
    await userEvent.click(within(item).getByRole('button'))

    expect(await screen.findByRole('alert')).toHaveTextContent('something went wrong')
  })

  it('show error when loading items after mark complete given error thrown', async () => {
    mockGetTodoItems
      .mockResolvedValueOnce(() => [{ id: '123' }, { id: '456' }, { id: '789' }])
      .mockRejectedValueOnce(new Error('something went wrong'))
    mockUpdateTodoItem.mockResolvedValueOnce({})

    render(<ItemList />)
    const item = await screen.findByTestId('item-456')
    await userEvent.click(within(item).getByRole('button'))

    expect(await screen.findByRole('alert')).toHaveTextContent('something went wrong')
  })
})
