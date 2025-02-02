import { Button, Container, Row, Col, Form, Stack, Alert } from 'react-bootstrap'
import React, { useState } from 'react'
import { v4 as uuidv4 } from 'uuid' //it may be a better idea to issue the id in the backend
import { createTodoItem } from '../services/api'

const AddItemForm = () => {
  const [description, setDescription] = useState('')
  const [error, setError] = useState('')

  const handleDescriptionChange = (event) => {
    setDescription(event.target.value)
    setError('')
  }

  async function handleAdd() {
    createTodoItem({ id: uuidv4(), description }) //the list needs to be refreshed manually to see the new item
      .then(handleClear)
      .catch((error) => setError(error.message))
  }

  function handleClear() {
    setDescription('')
    setError('')
  }

  return (
    <Container>
      <h1>Add Item</h1>
      <Form.Group as={Row} className="mb-3" controlId="formAddTodoItem">
        <Form.Label column sm="2">
          Description
        </Form.Label>
        <Col md="6">
          <Form.Control
            type="text"
            placeholder="Enter description..."
            value={description}
            onChange={handleDescriptionChange}
          />
        </Col>
      </Form.Group>
      <Form.Group as={Row} className="mb-3 offset-md-2" controlId="formAddTodoItem">
        <Stack direction="horizontal" gap={2}>
          <Button variant="primary" onClick={() => handleAdd()}>
            Add Item
          </Button>
          <Button variant="secondary" onClick={() => handleClear()}>
            Clear
          </Button>
        </Stack>
      </Form.Group>
      <Alert variant="danger" show={!!error}>
        {error}
      </Alert>
    </Container>
  )
}

export default AddItemForm
