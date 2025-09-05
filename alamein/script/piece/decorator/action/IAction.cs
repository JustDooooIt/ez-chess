using System.Threading.Tasks;

public interface IAction<T> where T : Event
{
  void ReciveEvent(T @event);
}