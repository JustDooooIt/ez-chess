public interface IPieceProvider<out T>
{
  T OriginPiece { get; }
}