import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import AddItemForm from '../AddItemForm'

const mockCreateTodoItem = jest.fn()

jest.mock('../../services/api.js', () => ({
  createTodoItem: (...args) => mockCreateTodoItem(...args),
}))

describe('AddItemForm should', () => {
  it('init with empty value', () => {
    render(<AddItemForm />)

    expect(screen.getByRole('textbox').value).toBe('')
  })

  it('display input value', async () => {
    render(<AddItemForm />)

    const input = screen.getByRole('textbox')
    await userEvent.type(input, 'test')

    await waitFor(() => {
      expect(input.value).toBe('test')
    })
  })

  it('call createToDoItem on add', async () => {
    render(<AddItemForm />)

    await userEvent.type(screen.getByRole('textbox'), 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))

    expect(mockCreateTodoItem).toHaveBeenCalledTimes(1)
    expect(mockCreateTodoItem).toHaveBeenCalledWith({
      id: expect.stringMatching(/^[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}$/i),
      description: 'test item',
    })
  })
})
