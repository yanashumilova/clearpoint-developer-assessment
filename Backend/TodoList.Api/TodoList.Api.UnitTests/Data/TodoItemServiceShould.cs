using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoList.Data;
using Xunit;

namespace TodoList.Api.UnitTests
{
  public class TodoItemServiceShould
  {

    private static TodoContext InitContext()
    {
      var _contextOptions = new DbContextOptionsBuilder<TodoContext>()
        .UseInMemoryDatabase(nameof(TodoItemService))
        //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

      var context = new TodoContext(_contextOptions);

      context.Database.EnsureDeleted();
      context.Database.EnsureCreated();

      return context;
    }

    [Fact]
    public async Task Return_AllPendingItems_When_GetAllPending()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = true,
        Description = "Completed",
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 2",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.GetAllPending();

      result.Should().NotBeNull();
      result.Should().HaveCount(2)
        .And.Contain(x => x.Description == "Pending 1")
        .And.Contain(x => x.Description == "Pending 2");
    }

    [Fact]
    public async Task Return_Item_When_Get()
    {
      var id = Guid.NewGuid();
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 2",
      });
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 3",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.Get(id);

      result.Should().NotBeNull();
      result.Id.Should().Be(id);
      result.Description.Should().Be("Pending 2");
      result.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task Return_Null_When_Get_Given_ItemDoesntExist()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.Get(Guid.NewGuid());

      result.Should().BeNull();
    }

    [Fact]
    public async Task Return_Null_When_Update_Given_ItemDoesntExist()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.SaveChanges();

      var id = Guid.NewGuid();
      var sut = new TodoItemService(context);

      var result = await sut.Update(new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      });

      result.Should().BeNull();
    }

    [Fact]
    public async Task Update_Item_When_Update()
    {
      var id = Guid.NewGuid();
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.Update(new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      });

      context.TodoItems.Should().HaveCount(1);
      var item = await context.TodoItems.SingleAsync();
      item.Id.Should().Be(id);
      item.IsCompleted.Should().BeTrue();
      item.Description.Should().Be("updated item");
    }

    [Fact]
    public async Task Return_Item_When_Update()
    {
      var id = Guid.NewGuid();
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = id,
        IsCompleted = false,
        Description = "Pending 1",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.Update(new TodoItem
      {
        Id = id,
        IsCompleted = true,
        Description = "updated item",
      });

      result.Should().NotBeNull();
      result.Id.Should().Be(id);
      result.IsCompleted.Should().BeTrue();
      result.Description.Should().Be("updated item");
    }

    [Fact]
    public async Task Throw_DuplicateDescriptionException_When_Create_GivenExistingDescription()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "new item",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var action = () => sut.Create(new TodoItem
      {
        IsCompleted = false,
        Description = "new item",
      });

      await action.Should().ThrowAsync<DuplicateDescriptionException>();
    }

    [Fact]
    public async Task Create_NewItem_When_PostTodoItem()
    {
      var context = InitContext();
      context.TodoItems.Add(new TodoItem
      {
        Id = Guid.NewGuid(),
        IsCompleted = false,
        Description = "old item",
      });
      context.SaveChanges();

      var sut = new TodoItemService(context);

      var result = await sut.Create(new TodoItem
      {
        IsCompleted = false,
        Description = "new item",
      });

      context.TodoItems.Should().HaveCount(2);
      context.TodoItems.Should().Contain(x => x.Description == "new item");
    }

    [Fact]
    public async Task Return_NewItem_When_PostTodoItem()
    {
      var context = InitContext();

      var sut = new TodoItemService(context);

      var result = await sut.Create(new TodoItem
      {
        IsCompleted = false,
        Description = "new item",
      });

      var savedItem = await context.TodoItems.SingleAsync();

      result.Should().NotBeNull();
      result.Id.Should().Be(savedItem.Id);
      result.IsCompleted.Should().BeFalse();
      result.Description.Should().Be("new item");
    }
  }
}
