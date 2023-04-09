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
    mockCreateTodoItem.mockResolvedValueOnce({})
    render(<AddItemForm />)

    await userEvent.type(screen.getByRole('textbox'), 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))

    expect(mockCreateTodoItem).toHaveBeenCalledTimes(1)
    expect(mockCreateTodoItem).toHaveBeenCalledWith({
      id: expect.stringMatching(/^[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}$/i),
      description: 'test item',
    })
  })

  it('show error when createToDoItem given error thrown', async () => {
    mockCreateTodoItem.mockRejectedValueOnce(new Error('something went wrong'))
    render(<AddItemForm />)

    await userEvent.type(screen.getByRole('textbox'), 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent('something went wrong')
  })

  it('clear error when typing', async () => {
    mockCreateTodoItem.mockRejectedValueOnce(new Error('something went wrong'))
    render(<AddItemForm />)

    const input = screen.getByRole('textbox')
    await userEvent.type(input, 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))
    await screen.findByRole('alert')
    await userEvent.type(input, ' ')

    await waitFor(() => {
      expect(screen.queryByRole('alert')).not.toBeInTheDocument()
    })
  })

  it('clear error when clear', async () => {
    mockCreateTodoItem.mockRejectedValueOnce(new Error('something went wrong'))
    render(<AddItemForm />)

    const input = screen.getByRole('textbox')
    await userEvent.type(input, 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))
    await screen.findByRole('alert')
    await userEvent.click(screen.getByRole('button', { name: /clear/i }))

    await waitFor(() => {
      expect(screen.queryByRole('alert')).not.toBeInTheDocument()
    })
  })

  it('clear error and input when succeeded', async () => {
    mockCreateTodoItem.mockRejectedValueOnce(new Error('something went wrong')).mockResolvedValueOnce({})
    render(<AddItemForm />)

    const input = screen.getByRole('textbox')
    await userEvent.type(input, 'test item')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))
    await screen.findByRole('alert')
    await userEvent.click(screen.getByRole('button', { name: /add/i }))

    await waitFor(() => {
      expect(screen.queryByRole('alert')).not.toBeInTheDocument()
    })
    expect(input.value).toBe('')
  })
})
