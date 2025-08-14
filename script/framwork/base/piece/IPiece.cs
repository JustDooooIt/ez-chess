using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject Origin { get; }
  IInterfaceQueryable Proxy { get; }
  IInterfaceQueryable Wrapper { get; }
  PipelineAdapter PipelineAdapter { get; set; }
  PiecesManager PiecesManager { get; set; }
  ulong GetInstanceId();

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
    else if (wrapper is PieceDecorator<IPiece> decorator)
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
}