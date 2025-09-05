using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public partial class PipelineEventBus
{
  private static readonly Lazy<PipelineEventBus> instance = new(() => new PipelineEventBus());
  private readonly Dictionary<(Type type, ulong valveId), List<Delegate>> eventListeners = [];

  public static PipelineEventBus Instance => instance.Value;

  public void Subscribe<T>(ulong valveId, Action<T> listener) where T : Event
  {
    var key = (typeof(T), valveId);

    if (!eventListeners.TryGetValue(key, out List<Delegate> value))
    {
      value = [];
      eventListeners[key] = value;
    }

    value.Add(listener);
  }

  public void Publish<T>(ulong valveId, T eventData) where T : Event
  {
    var key = (typeof(T), valveId);
    if (eventListeners.TryGetValue(key, out List<Delegate> listeners))
    {
      foreach (var listener in listeners)
      {
        ((Action<T>)listener).Invoke(eventData);
      }
    }
  }
}

public abstract record Event
{
  
}