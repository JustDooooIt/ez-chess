using System.Collections.Generic;

public interface IMultiFaceted<T>
{
  T CurrentState { get; }
}