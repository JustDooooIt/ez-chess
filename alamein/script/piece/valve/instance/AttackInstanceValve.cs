using System.Threading.Tasks;

public partial class AttackInstanceValve(IPieceInstance pieceInstance, AttackEvent @event) : InstanceValve(pieceInstance)
{
  protected override void DoLaunch()
  {
    pieceInstance.As<IAttackable>()?.ReciveEvent(@event);
  }
}