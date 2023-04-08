using System;

namespace TodoList.Data
{
  /*TODO: 
  The entity is not setup with PK and NULL/NOT NULL
  This can be done with DataAnnotation attributes or, preverably, with FluentAPI
  Depending on what DB is used, it could be more efficient to set Unique constraint on Description rather than checking it in code
  */
  public class TodoItem
  {
    public Guid Id { get; set; }

    public string Description { get; set; }

    public bool IsCompleted { get; set; }
  }
}
