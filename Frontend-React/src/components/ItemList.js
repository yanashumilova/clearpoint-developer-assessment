import { Alert, Button, Table } from 'react-bootstrap'
import React, { useState, useEffect } from 'react'
import { getTodoItems, updateTodoItem } from '../services/api'

const ItemList = () => {
  const [items, setItems] = useState([])
  const [error, setError] = useState('')

  async function getItems() {
    setError('')
    await getTodoItems()
      .then(setItems)
      .catch((error) => setError(error.message))
  }

  useEffect(() => {
    getItems().catch((error) => setError(error.message))
  }, [])

  async function handleMarkAsComplete(item) {
    // TODO: it may be preferable to have a targeted "markCompleted" endpoint instead
    //       of using generic update to avoid unexpected changes to other fields
    await updateTodoItem({ ...item, isCompleted: true })
      //if the lists are expected to be very long or items to contain a lot of data
      //it is better to optimistically update the list locally instead of pulling all items
      //pagination would need to be implemented as well in that case
      .then(getItems)
      .catch((error) => setError(error.message))
  }

  return (
    <>
      <h1>
        Showing {items.length} Item(s){' '}
        <Button variant="primary" className="pull-right" onClick={() => getItems()}>
          Refresh
        </Button>
      </h1>
      <Alert variant="danger" show={!!error}>
        {error}
      </Alert>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Id</th>
            <th>Description</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id} data-testid={`item-${item.id}`}>
              <td>{item.id}</td>
              <td>{item.description}</td>
              <td>
                <Button variant="warning" size="sm" onClick={() => handleMarkAsComplete(item)}>
                  Mark as completed
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  )
}

export default ItemList
