public interface IInterfaceQueryable
{
  public virtual T As<T>() where T : class
  {
    if (this is T selfImpl) return selfImpl;

    return null;
  }
}
