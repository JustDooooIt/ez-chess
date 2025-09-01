using System.Threading.Tasks;

public interface IAction<in T> where T : Event
{
  void ReciveEvent(T @event);
}