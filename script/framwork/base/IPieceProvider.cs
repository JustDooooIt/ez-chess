public interface IPieceProvider<out T>
{
  T Wrapped { get; }
}