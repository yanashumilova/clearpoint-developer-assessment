using System;

namespace TodoList.Data
{
  public class DuplicateDescriptionException: ApplicationException {
    public DuplicateDescriptionException() : base("Description already exists") {}
  }
}
