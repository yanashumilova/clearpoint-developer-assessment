using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Core;

namespace TodoList.Data
{
  public class TodoItemService: ITodoItemService
  {
    private readonly TodoContext _context;

    public TodoItemService(TodoContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<TodoItemModel>> GetAllPending()
    {
      var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
      return results.Select(ToModel).ToList();
    }

    public async Task<TodoItemModel> Get(Guid id)
    {
      var result = await _context.TodoItems.FindAsync(id);

      return ToModel(result);
    }

    public async Task<TodoItemModel> Update(TodoItemModel todoItem)
    {
      var savedItem = await _context.TodoItems.Where(x => x.Id == todoItem.Id).SingleOrDefaultAsync();
      if (savedItem is null)
      {
        return null;
      }

      if (savedItem.Description != todoItem.Description && TodoItemDescriptionExists(todoItem.Description))
      {
        throw new DuplicateDescriptionException();
      }

      savedItem.IsCompleted = todoItem.IsCompleted;
      savedItem.Description = todoItem.Description;

      await _context.SaveChangesAsync();

      return ToModel(savedItem);
    }

    public async Task<TodoItemModel> Create(TodoItemModel todoItem)
    {
      if (TodoItemDescriptionExists(todoItem.Description)) {
        throw new DuplicateDescriptionException();
      }

      var savedItem = new TodoItem
      {
        Id = todoItem.Id,
        IsCompleted = todoItem.IsCompleted,
        Description = todoItem.Description,
      };
      _context.TodoItems.Add(savedItem);
      await _context.SaveChangesAsync();

      return ToModel(savedItem);
    }

    private bool TodoItemDescriptionExists(string description)
    {
      return _context.TodoItems
             .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
    }

    // could use a library such as AutoMapper or Mapster
    // however this is not too many fields, so the use is not justified
    // even with large set of fields it may be better to have custom mappers as it is frequently found that the data models, 
    // core models and api models all have individual context requirements and do not translate one for one
    private TodoItemModel ToModel(TodoItem item) {
      if (item is null) return null;
      
      return new TodoItemModel
      {
        Id = item.Id,
        IsCompleted = item.IsCompleted,
        Description = item.Description,
      };
    }
  }
}
