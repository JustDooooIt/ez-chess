using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public partial class PipelineEventBus
{
  private static readonly Lazy<PipelineEventBus> instance = new(() => new PipelineEventBus());
  private readonly Dictionary<(Type type, ulong valveId), List<Delegate>> eventListeners = [];

  public static PipelineEventBus Instance => instance.Value;

  public void Subscribe<T>(ulong valveId, Action<T> listener)
  {
    var key = (typeof(T), valveId);

    if (!eventListeners.TryGetValue(key, out List<Delegate> value))
    {
      value = [];
      eventListeners[key] = value;
    }

    value.Add(listener);
  }

  public void Publish<T>(ulong valveId, T eventData)
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

public record RenderEvent : Event
{
  public int valveId;
  public ulong pieceId;
}

public record MoveEvent : RenderEvent
{
  public Vector2I from;
  public Vector2I to;

  public MoveEvent(Vector2I from, Vector2I to)
  {
    this.from = from;
    this.to = to;
  }
}