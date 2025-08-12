using Godot;

public abstract class PieceFactoryBase : IPieceFactory
{
  protected abstract string PieceAdapterScenePath{ get; }
  
  protected abstract string PieceInstanceScenePath{ get; }

  public abstract PieceAdapter Create(int pieceType, params Variant[] args);
}