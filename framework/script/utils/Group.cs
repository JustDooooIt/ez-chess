using System.Collections.Generic;
using Godot;

public class Group<K, V>
{
  private readonly Dictionary<K, List<V>> _dict = [];

  public Dictionary<K, List<V>> Wrapped => _dict;

  public void Add(K key, V value)
  {
    if (_dict.TryGetValue(key, out var values))
    {
      values.Add(value);
    }
    else
    {
      _dict.Add(key, [value]);
    }
  }

  public List<V> Get(K key)
  {
    if (_dict.TryGetValue(key, out var values))
    {
      return values;
    }
    else
    {
      _dict[key] = [];
      return _dict[key];
    }
  }
}