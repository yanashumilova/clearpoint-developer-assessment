using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Data
{
  public interface ITodoItemService
  {
    Task<IEnumerable<TodoItem>> GetAllPending();
    Task<TodoItem> Get(Guid id);
    Task<TodoItem> Update(TodoItem todoItem);
    Task<TodoItem> Create(TodoItem todoItem);
  }
}
