using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Core;

namespace TodoList.Data
{
  public static class RegisterServices
  {
    public static IServiceCollection RegisterDataServices(this IServiceCollection services)
    {
      services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoItemsDB"));
      services.AddScoped<ITodoItemService, TodoItemService>();
      return services;
    }
  }
}
