using System;

public abstract class PieceDecorator(IPiece proxy) : IPiece
{
  public event Action<int> ActionCompleted;
  
  protected IPiece _proxy = proxy;
}