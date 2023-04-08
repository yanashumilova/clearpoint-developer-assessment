using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Core;

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
    public async Task<IActionResult> PutTodoItem(Guid id, TodoItemModel TodoItemModel)
    {
      if (id != TodoItemModel.Id)
      {
        return BadRequest();
      }

      try
      {
        var result = await _dataService.Update(TodoItemModel);
        if (result is null)
        {
          return NotFound();
        }
      }
      catch(DuplicateDescriptionException) {
        return BadRequest("Description already exists");
      }

      return NoContent();
    }

    // POST: api/TodoItems 
    [HttpPost]
    public async Task<IActionResult> PostTodoItem(TodoItemModel TodoItemModel)
    {
      if (string.IsNullOrEmpty(TodoItemModel?.Description))
      {
        return BadRequest("Description is required");
      }

      try
      {
        var result = await _dataService.Create(TodoItemModel);
        return CreatedAtAction(nameof(GetTodoItem), new { id = TodoItemModel.Id }, TodoItemModel);
      }
      catch (DuplicateDescriptionException) {
        return BadRequest("Description already exists");
      }
    }
  }
}
