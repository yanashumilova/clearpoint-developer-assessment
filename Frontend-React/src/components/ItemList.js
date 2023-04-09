import { Button, Table } from 'react-bootstrap'
import React, { useState, useEffect } from 'react'
import { getTodoItems } from '../services/api'

const ItemList = () => {
  const [items, setItems] = useState([])
  console.log(items)
  async function getItems() {
    try {
      await getTodoItems().then(setItems)
    } catch (error) {
      console.error(error)
    }
  }

  useEffect(() => {
    getItems()
  }, [])

  async function handleMarkAsComplete(item) {
    try {
      alert('todo')
    } catch (error) {
      console.error(error)
    }
  }

  return (
    <>
      <h1>
        Showing {items.length} Item(s){' '}
        <Button variant="primary" className="pull-right" onClick={() => getItems()}>
          Refresh
        </Button>
      </h1>

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
            <tr key={item.id}>
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
