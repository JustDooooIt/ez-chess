using System;

public abstract class PieceDecorator(IPiece wrapped) : IPiece, IInterfaceQueryable
{
  public event Action<int> ActionCompleted;

  protected IPiece _wrapped = wrapped;

  public virtual T As<T>() where T : class
  {
    if (this is T selfImpl) return selfImpl;

    if (_wrapped is T wrappedImpl) return wrappedImpl;

    if (_wrapped is IInterfaceQueryable queryable)
      return queryable.As<T>();

    return null;
  }
}

public interface IInterfaceQueryable
{
  T As<T>() where T : class;
}
