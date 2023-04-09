import { render, screen, waitFor } from '@testing-library/react'
import ItemList from '../ItemList'
import userEvent from '@testing-library/user-event'

const mockGetTodoItems = jest.fn()

jest.mock('../../services/api.js', () => ({
  getTodoItems: (...args) => mockGetTodoItems(...args),
}))

describe('ItemList should', () => {
  it('load items on mount', async () => {
    mockGetTodoItems.mockResolvedValueOnce(() => [{}, {}, {}])

    render(<ItemList />)

    expect(mockGetTodoItems).toHaveBeenCalledTimes(1)
    expect(mockGetTodoItems).toHaveBeenCalledWith()

    await waitFor(() => {
      expect(screen.getByText(/3 item/i)).toBeInTheDocument()
    })
  })

  it('load items on refresh', async () => {
    mockGetTodoItems.mockResolvedValueOnce(() => [{}, {}, {}]).mockResolvedValueOnce(() => [{}, {}, {}, {}])

    render(<ItemList />)

    await screen.findByText(/3 item/i)

    await userEvent.click(screen.getByRole('button', { name: /refresh/i }))

    expect(mockGetTodoItems).toHaveBeenCalledTimes(2)
    expect(mockGetTodoItems).toHaveBeenLastCalledWith()

    await waitFor(() => {
      expect(screen.getByText(/4 item/i)).toBeInTheDocument()
    })
  })
})
