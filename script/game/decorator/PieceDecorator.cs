using System;
using Godot;

public abstract class PieceDecorator<T>(IPiece wrapped) : IPiece, IInterfaceQueryable, IPieceProvider<T>
{
  public event Action<int> ActionCompleted;

  protected IPiece _wrapped = wrapped;

  public T Piece => (T)_wrapped;
  public Node Origin { get => GetOrigin(); }

  public virtual V As<V>() where V : class
  {
    if (this is V selfImpl) return selfImpl;

    if (_wrapped is V wrappedImpl) return wrappedImpl;

    if (_wrapped is IInterfaceQueryable queryable)
      return queryable.As<V>();

    return null;
  }

  private Node GetOrigin()
  {
    if (_wrapped is PieceDecorator<T> decorator)
    {
      return decorator.GetOrigin();
    }
    else if (_wrapped is Node)
    {
      return _wrapped as Node;
    }
    return null;
  }

}