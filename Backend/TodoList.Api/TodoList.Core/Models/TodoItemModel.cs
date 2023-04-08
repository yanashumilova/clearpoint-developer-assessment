using System;

namespace TodoList.Core
{
  // the core model is introduced to break link between the data and API layers
  // exposing data models into the API is not a good idea as, it creates unnecessary
  // dependancies, mixes concerns and, in case of EF and depending on configuration, 
  // can result in unexpected DB request
  public class TodoItemModel
  {
    public Guid Id { get; set; }

    public string Description { get; set; }

    public bool IsCompleted { get; set; }
  }
}
