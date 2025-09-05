using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class AttackStateDecorator(IPieceState wrapped, float attack) : PieceStateDecorator(wrapped), IAttackable, IAttackEventSender, IAttackPointProvider
{
  public float AttackPoint { get; set; } = attack;

  public void ReciveEvent(AttackEvent @event)
  {
    
    if (!@event.recovered && PipelineAdapter is not OtherPipeline)
    {
      var fromPiece = PieceAdapter.GameManager.GetPiece(@event.fromFaction, @event.fromPiece);
      var toPiece = PieceAdapter.GameManager.GetPiece(@event.targetFaction, @event.targetPiece);
      CombatController.Instance.AddCombatUnit(fromPiece.GetInstanceId(), toPiece.GetInstanceId());
      PieceAdapter.Instance.IsRunning = true;
    }
  }

  public void SendAttackEvent(Vector2I target, PieceAdapter targetPiece)
  {
    Vector2I from = PieceAdapter.State.As<IPositionable>().MapPosition;
    var @event = new AttackEvent(from, PieceAdapter.Faction, PieceAdapter.Name, target, targetPiece.Faction, targetPiece.Name);
    Valve valve = new AttackStateValve(this, @event);
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<AttackEvent>(valve);
  }
}