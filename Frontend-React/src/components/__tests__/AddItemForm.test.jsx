import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import AddItemForm from '../AddItemForm'

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
})
