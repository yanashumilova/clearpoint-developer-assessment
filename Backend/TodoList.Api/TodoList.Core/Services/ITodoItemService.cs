using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Core
{
  public interface ITodoItemService
  {
    Task<IEnumerable<TodoItemModel>> GetAllPending();
    Task<TodoItemModel> Get(Guid id);
    Task<TodoItemModel> Update(TodoItemModel todoItem);
    Task<TodoItemModel> Create(TodoItemModel todoItem);
  }
}
