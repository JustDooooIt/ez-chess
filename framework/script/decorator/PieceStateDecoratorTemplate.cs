using System;
using Godot;

public abstract partial class PieceStateDecorator<E>(IPieceState wrapped) : PieceDecorator<E>(wrapped), IPieceState where E : PieceEvent
{
  private IPieceState _wrapped = wrapped;
  public PieceAdapter PieceAdapter { get => _wrapped.PieceAdapter; set => _wrapped.PieceAdapter = value; }

  protected void AddValve<T, V>(E @event)
    where V : Valve
    where T : Event
  {
    object[] args = [this, @event];
    Valve valve = (Valve)Activator.CreateInstance(typeof(V), args);
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<T>(valve);
  }
}