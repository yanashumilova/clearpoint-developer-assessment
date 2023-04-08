using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TodoList.Api.Controllers;
using TodoList.Core;
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
        new TodoItemModel
        {
          Id = Guid.NewGuid(),
          IsCompleted = false,
          Description = "Pending 1",
        },
        new TodoItemModel
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
      okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItemModel>>();

      var items = okResult.Value as IEnumerable<TodoItemModel>;
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
      dataService.Get(id).Returns(new TodoItemModel
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
      okResult.Value.Should().BeOfType<TodoItemModel>();

      var item = okResult.Value as TodoItemModel;
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
      dataService.Get(id).Returns(Task.FromResult<TodoItemModel>(null));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItem(Guid.NewGuid());

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Return_BadRequest_When_PutTodoItem_Given_MismatchedIdInInput()
    {
      var sut = new TodoItemsController(Substitute.For<ITodoItemService>(), Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(Guid.NewGuid(), new TodoItemModel
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
      var TodoItemModel = new TodoItemModel
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Update(TodoItemModel).Returns(Task.FromResult<TodoItemModel>(null));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, TodoItemModel);

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Return_BadRequest_When_PutTodoItem_Given_DataServiceThrowsDuplicateDescriptionException()
    {
      var id = Guid.NewGuid();
      var TodoItemModel = new TodoItemModel
      {
        Id = id,
        IsCompleted = false,
        Description = "new item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Update(TodoItemModel).Returns(Task.FromException<TodoItemModel>(new DuplicateDescriptionException()));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, TodoItemModel);

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestObjectResult>();

      var badRequestResult = result as BadRequestObjectResult;
      badRequestResult.Value.Should().Be("Description already exists");
    }

    [Fact]
    public async Task Update_Item_When_PutTodoItem()
    {
      var id = Guid.NewGuid();
      var TodoItemModel = new TodoItemModel
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Update(TodoItemModel).Returns(TodoItemModel);

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, TodoItemModel);

      await dataService.Received(1).Update(TodoItemModel);

      result.Should().NotBeNull();
      result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Return_BadRequest_When_PostTodoItem_Given_EmptyDescription()
    {
      var sut = new TodoItemsController(Substitute.For<ITodoItemService>(), Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(new TodoItemModel
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
    public async Task Return_BadRequest_When_PostTodoItem_Given_DataServiceThrowsDuplicateDescriptionException()
    {
      var id = Guid.NewGuid();
      var TodoItemModel = new TodoItemModel
      {
        Id = id,
        IsCompleted = false,
        Description = "new item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Create(TodoItemModel).Returns(Task.FromException<TodoItemModel>(new DuplicateDescriptionException()));

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(TodoItemModel);

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestObjectResult>();

      var badRequestResult = result as BadRequestObjectResult;
      badRequestResult.Value.Should().Be("Description already exists");
    }

    [Fact]
    public async Task Create_NewItem_When_PostTodoItem()
    {
      var id = Guid.NewGuid();
      var TodoItemModel = new TodoItemModel
      {
        Id = id,
        IsCompleted = false,
        Description = "new item",
      };
      var dataService = Substitute.For<ITodoItemService>();
      dataService.Create(TodoItemModel).Returns(TodoItemModel);

      var sut = new TodoItemsController(dataService, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PostTodoItem(TodoItemModel);

      result.Should().NotBeNull();
      result.Should().BeOfType<CreatedAtActionResult>();

      var createdAtActionResult = result as CreatedAtActionResult;
      createdAtActionResult.ActionName.Should().Be("GetTodoItem");
      createdAtActionResult.RouteValues.Should().HaveCount(1)
        .And.ContainKey("id");
      createdAtActionResult.RouteValues["id"].Should().Be(id);
      createdAtActionResult.Value.Should().BeOfType<TodoItemModel>();

      var item = createdAtActionResult.Value as TodoItemModel;
      item.Should().NotBeNull();
      item.Id.Should().Be(id);
      item.IsCompleted.Should().BeFalse();
      item.Description.Should().Be("new item");
    }
  }
}
