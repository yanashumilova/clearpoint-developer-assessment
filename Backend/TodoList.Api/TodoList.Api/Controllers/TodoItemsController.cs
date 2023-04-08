using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Data;

namespace TodoList.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TodoItemsController : ControllerBase
  {
    private readonly ITodoItemService _dataService;
    private readonly ILogger<TodoItemsController> _logger;

    public TodoItemsController(ITodoItemService dataService, ILogger<TodoItemsController> logger)
    {
      _dataService = dataService;
      _logger = logger;
    }

    // GET: api/TodoItems
    [HttpGet]
    public async Task<IActionResult> GetTodoItems()
    {
      var results = await _dataService.GetAllPending();
      return Ok(results);
    }

    // GET: api/TodoItems/...
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoItem(Guid id)
    {
      var result = await _dataService.Get(id);

      if (result == null)
      {
        return NotFound();
      }

      return Ok(result);
    }

    // PUT: api/TodoItems/... 
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
    {
      if (id != todoItem.Id)
      {
        return BadRequest();
      }

      var result = await _dataService.Update(todoItem);
      if (result is null)
      {
        return NotFound();
      }

      return NoContent();
    }

    // POST: api/TodoItems 
    [HttpPost]
    public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
    {
      if (string.IsNullOrEmpty(todoItem?.Description))
      {
        return BadRequest("Description is required");
      }

      try
      {
        var result = await _dataService.Create(todoItem);
        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
      }
      catch (DuplicateDescriptionException) {
        return BadRequest("Description already exists");
      }
    }
  }
}
