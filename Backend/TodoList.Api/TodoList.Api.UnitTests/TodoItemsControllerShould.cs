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
        [Fact]
        public async Task Return_AllIncompleteItems_When_GetTodoItems()
        {
            var _contextOptions = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(nameof(TodoItemsController))
                //.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new TodoContext(_contextOptions);


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
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>();

            var items = okResult.Value as IEnumerable<TodoItem>;
            items.Should().HaveCount(2)
                .And.Contain(x => x.Description == "Pending 1")
                .And.Contain(x => x.Description == "Pending 2");
        }
    }
}
