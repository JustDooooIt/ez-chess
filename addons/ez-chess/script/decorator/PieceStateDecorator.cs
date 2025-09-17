using System;
using Godot;

public abstract partial class PieceStateDecorator(IPieceState wrapped) : PieceDecorator(wrapped), IPieceState{
  private IPieceState _wrapped = wrapped;
  public PieceAdapter PieceAdapter { get => _wrapped.PieceAdapter; set => _wrapped.PieceAdapter = value; }

  protected void AddValve<E, V>(E @event)
    where V : Valve
    where E : Event
  {
    object[] args = [this, @event];
    Valve valve = (Valve)Activator.CreateInstance(typeof(V), args);
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<E>(valve);
  }
}