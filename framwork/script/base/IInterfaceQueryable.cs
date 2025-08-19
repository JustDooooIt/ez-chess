using System.Collections.Generic;
using Godot;

public interface IInterfaceQueryable
{

  GodotObject Origin { get; }
  IPiece Wrapped { get; }
  IInterfaceQueryable Proxy { get; }
  IInterfaceQueryable Wrapper { get; }

  public void SetWrapper(IPiece wrapper)
  {
    if (wrapper is PieceState pieceState)
    {
      pieceState.Wrapper = this;
    }
    else if (wrapper is PieceInstance pieceInstance)
    {
      pieceInstance.Wrapper = this;
    }
    else if (wrapper is PieceDecorator decorator)
    {
      decorator.Wrapper = this;
    }
  }

  public IInterfaceQueryable GetProxy()
  {
    var piece = (IPiece)Wrapper;
    do
    {
      if (piece.Wrapper == null)
        return piece;
      else
        piece = (IPiece)piece.Wrapper;
    } while (true);
  }

  public virtual T As<T>() where T : class
  {
    if (this is T selfImpl) return selfImpl;

    return null;
  }

  IEnumerable<T> QueryAll<T>() where T : class
  {
    if (this is T self)
    {
      yield return self;
    }

    if (Wrapped != null)
    {
      foreach (T item in Wrapped.QueryAll<T>())
      {
        yield return item;
      }
    }
  }

  public void RemoveSelf()
  {
    Wrapped.SetWrapper((IPiece)Wrapper);
  }
}
