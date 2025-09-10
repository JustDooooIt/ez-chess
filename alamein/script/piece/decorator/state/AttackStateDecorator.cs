using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class AttackStateDecorator(IPieceState wrapped, float attack) : PieceStateDecorator<AttackEvent>(wrapped), IAttackable, IAttackEventSender, IAttackPointProvider
{
  public float AttackPoint { get; set; } = attack;

  public void SendAttackEvent(Vector2I target, PieceAdapter targetPiece)
  {
    Vector2I from = PieceAdapter.State.As<IPositionable>().MapPosition;
    var @event = new AttackEvent(from, PieceAdapter.Faction, PieceAdapter.Name, target, targetPiece.Faction, targetPiece.Name);
    Valve valve = new AttackStateValve(this, @event);
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<AttackEvent>(valve);
  }

  protected override void DoReciveEvent(AttackEvent @event)
  {
    
  }

  protected override void SaveOperation(AttackEvent @event)
  {
    var fromPiece = PieceAdapter.GameManager.GetPiece(@event.fromFaction, @event.fromPiece);
    var toPiece = PieceAdapter.GameManager.GetPiece(@event.targetFaction, @event.targetPiece);
    CombatController.Instance.AddCombatUnit(fromPiece.GetInstanceId(), toPiece.GetInstanceId());
    PieceAdapter.Instance.IsRunning = true;
  }
}