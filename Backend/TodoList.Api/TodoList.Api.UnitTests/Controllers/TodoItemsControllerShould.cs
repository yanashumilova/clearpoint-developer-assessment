using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TodoList.Api.Controllers;
using TodoList.Data;
using Xunit;

namespace TodoList.Api.UnitTests
{
  public class TodoItemsControllerShould
  {
    [Fact]
    public async Task Return_AllPendingItems_When_GetTodoItems()
    {
      var dataService = Substitute.For<ITodoItemService>();
      dataService.GetAllPending().Returns(new[] {
        new TodoItem
        {
          Id = Guid.NewGuid(),
          IsCompleted = false,
          Description = "Pending 1",
        },
        new TodoItem
        {
          Id = Guid.NewGuid(),
          IsCompleted = false,
          Description = "Pending 2",
        }
      });

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItems();

      result.Should().NotBeNull();
      result.Should().BeOfType<OkObjectResult>();

      var okResult = result as OkObjectResult;
      okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>();

      var items = okResult.Value as IEnumerable<TodoItem>;
      items.Should().NotBeNull();
      items.Should().HaveCount(2)
          .And.Contain(x => x.Description == "Pending 1")
          .And.Contain(x => x.Description == "Pending 2");
    }

    [Fact]
    public async Task Return_Item_When_GetTodoItem()
    {
      var id = Guid.NewGuid();
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Get(id).Returns(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 2",
      });

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItem(id);

      result.Should().NotBeNull();
      result.Should().BeOfType<OkObjectResult>();

      var okResult = result as OkObjectResult;
      okResult.Value.Should().BeOfType<TodoItem>();

      var item = okResult.Value as TodoItem;
      item.Should().NotBeNull();
      item.Id.Should().Be(id);
      item.Description.Should().Be("Pending 2");
      item.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Return_NotFound_When_GetTodoItem_Given_DataServiceReturnsNull()
    {
      var id = Guid.NewGuid();
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Get(id).Returns(Task.FromResult<TodoItem>(null));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItem(Guid.NewGuid());

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Return_BadRequest_When_PutTodoItem_Given_MismatchedIdInInput()
    {
      var sut = new TodoItemsController(Substitute.For<ITodoItemService>(), Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(Guid.NewGuid(), new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = true,
        Description = "updated item",
      });

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Return_NotFound_When_PutTodoItem_Given_DataServiceReturnsNull()
    {
      var id = Guid.NewGuid();
      var todoItem = new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Update(todoItem).Returns(Task.FromResult<TodoItem>(null));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, todoItem);

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_Item_When_PutTodoItem()
    {
      var id = Guid.NewGuid();
      var todoItem = new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Update(todoItem).Returns(todoItem);

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, todoItem);

      await dataService.Received(1).Update(todoItem);

      result.Should().NotBeNull();
      result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Return_BadRequest_When_PostTodoItem_GivenEmptyDescription()
    {
      var sut = new TodoItemsController(Substitute.For<ITodoItemService>(), Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
      });

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestObjectResult>();

      var badRequestResult = result as BadRequestObjectResult;
      badRequestResult.Value.Should().Be("Description is required");
    }

    [Fact]
    public async Task Return_BadRequest_When_PostTodoItem_DataServiceThrowsDuplicateDescriptionException()
    {
      var id = Guid.NewGuid();
      var todoItem = new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "new item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Create(todoItem).Returns(Task.FromException<TodoItem>(new DuplicateDescriptionException()));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(todoItem);

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestObjectResult>();

      var badRequestResult = result as BadRequestObjectResult;
      badRequestResult.Value.Should().Be("Description already exists");
    }

    [Fact]
    public async Task Create_NewItem_When_PostTodoItem()
    {
      var id = Guid.NewGuid();
      var todoItem = new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "new item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Create(todoItem).Returns(todoItem);

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(todoItem);

      result.Should().NotBeNull();
      result.Should().BeOfType<CreatedAtActionResult>();

      var createdAtActionResult = result as CreatedAtActionResult;
      createdAtActionResult.ActionName.Should().Be("GetTodoItem");
      createdAtActionResult.RouteValues.Should().HaveCount(1)
        .And.ContainKey("id");
      createdAtActionResult.RouteValues["id"].Should().Be(id);
      createdAtActionResult.Value.Should().BeOfType<TodoItem>();

      var item = createdAtActionResult.Value as TodoItem;
      item.Should().NotBeNull();
      item.Id.Should().Be(id);
      item.IsCompleted.Should().BeFalse();
      item.Description.Should().Be("new item");
    }
  }
}
