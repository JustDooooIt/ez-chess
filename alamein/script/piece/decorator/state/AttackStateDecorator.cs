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
      CombatController.Instance.AddCombatUnit(@event.pieceId, @event.targetPiece);
      PieceAdapter.Instance.IsRunning = true;
      GD.Print("攻击成功");
    }
  }

  public void SendAttackEvent(Vector2I target, PieceAdapter targetPiece)
  {
    ulong pieceId = GetPieceId();
    Vector2I from = PieceAdapter.State.As<IPositionable>().MapPosition;
    var @event = new AttackEvent(from, pieceId, target, targetPiece.GetInstanceId());
    Valve valve = new AttackStateValve(this, @event);
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<AttackEvent>(valve);
  }
}