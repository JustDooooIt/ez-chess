public interface IPieceProvider<out T>
{
  T Piece { get; }
}