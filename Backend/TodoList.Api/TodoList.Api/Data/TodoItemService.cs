using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Data
{
  //TODO: should be moved out to a separate project along with EF, data context and data models
  public class TodoItemService: ITodoItemService
  {
    private readonly TodoContext _context;

    public TodoItemService(TodoContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<TodoItem>> GetAllPending()
    {
      var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
      return results;
    }

    public async Task<TodoItem> Get(Guid id)
    {
      var result = await _context.TodoItems.FindAsync(id);

      return result;
    }

    public async Task<TodoItem> Update(TodoItem todoItem)
    {
      var savedItem = await _context.TodoItems.Where(x => x.Id == todoItem.Id).SingleOrDefaultAsync();
      if (savedItem is null)
      {
        return null;
      }

      savedItem.IsCompleted = todoItem.IsCompleted;
      savedItem.Description = todoItem.Description;

      await _context.SaveChangesAsync();

      return savedItem;
    }

    public async Task<TodoItem> Create(TodoItem todoItem)
    {
      if (TodoItemDescriptionExists(todoItem.Description)) {
        throw new DuplicateDescriptionException();
      }

      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      return todoItem;
    }

    private bool TodoItemDescriptionExists(string description)
    {
      return _context.TodoItems
             .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
    }
  }
}
