using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TodoList.Api.Controllers;
using Xunit;

namespace TodoList.Api.UnitTests
{
  public class TodoItemsControllerShould
  {

    private static TodoContext InitContext()
    {
      var _contextOptions = new DbContextOptionsBuilder<TodoContext>()
        .UseInMemoryDatabase(nameof(TodoItemsController))
        //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

      var context = new TodoContext(_contextOptions);

      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();

      return context;
    }

    [Fact]
    public async Task Return_AllIncompleteItems_When_GetTodoItems()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1"
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = true,
        Description = "Completed"
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 2"
      });
      context.SaveChanges();

      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItems();

      result.Should().NotBeNull();
      result.Should().BeOfType<OkObjectResult>();

      var okResult = result as OkObjectResult;
      okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>();

      var items = okResult.Value as IEnumerable<TodoItem>;
      items.Should().HaveCount(2)
          .And.Contain(x => x.Description == "Pending 1")
          .And.Contain(x => x.Description == "Pending 2");
    }

    [Fact]
    public async Task Return_Item_When_GetTodoItem()
    {
      var id = Guid.NewGuid();
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1"
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 2"
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 3"
      });
      context.SaveChanges();

      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItem(id);

      result.Should().NotBeNull();
      result.Should().BeOfType<OkObjectResult>();

      var okResult = result as OkObjectResult;
      okResult.Value.Should().BeOfType<TodoItem>();

      var item = okResult.Value as TodoItem;
      item.Id.Should().Be(id);
      item.Description.Should().Be("Pending 2");
      item.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Return_404_When_GetTodoItem_Given_ItemDoesntExist()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1"
      });
      context.SaveChanges();

      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.GetTodoItem(Guid.NewGuid());

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Return_400_When_PutTodoItem_Given_MismatchedIdInImput()
    {
      var context = InitContext();
      
      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(Guid.NewGuid(), new TodoItem { 
        Id = Guid.NewGuid(),
        IsCompleted = true,
        Description = "updated item"
      });

      result.Should().NotBeNull();
      result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Return_404_When_PutTodoItem_Given_ItemDoesntExist()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1"
      });
      context.SaveChanges();

      var id = Guid.NewGuid();
      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item"
      });

      result.Should().NotBeNull();
      result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_Item_When_PutTodoItem()
    {
      var id = Guid.NewGuid();
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 1"
      });
      context.SaveChanges();

      var sut = new TodoItemsController(context, Substitute.For<ILogger<TodoItemsController>>());

      var result = await sut.PutTodoItem(id, new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item"
      });

      context.TodoItems.Should().HaveCount(1);
      var item = await context.TodoItems.SingleAsync();
      item.Id.Should().Be(id);
      item.IsCompleted.Should().BeTrue();
      item.Description.Should().Be("updated item");

      result.Should().NotBeNull();
      result.Should().BeOfType<NoContentResult>();
    }
  }
}
