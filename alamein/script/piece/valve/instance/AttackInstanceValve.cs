using System.Threading.Tasks;

public partial class AttackInstanceValve(IPieceInstance pieceInstance, AttackEvent @event) : InstanceValve(pieceInstance, @event)
{
  protected override void DoLaunch()
  {
    _pieceInstance.Query<IAttackable>()?.ReciveEvent(@event);
  }
}