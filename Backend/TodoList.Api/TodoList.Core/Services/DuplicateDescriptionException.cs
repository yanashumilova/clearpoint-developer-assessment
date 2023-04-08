using System;

namespace TodoList.Core
{
  public class DuplicateDescriptionException: ApplicationException {
    public DuplicateDescriptionException() : base("Description already exists") {}
  }
}
