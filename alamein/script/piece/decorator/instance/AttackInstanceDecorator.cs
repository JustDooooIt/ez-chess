using Godot;

[RegisterValve("Attack", ValveTypes.INSTANCE)]
public partial class AttackInstanceDecorator : PieceInstanceDecorator<AttackEvent>, IAttackable
{
  private IPieceInstance _wrapped;

  public AttackInstanceDecorator(IPieceInstance wrapped) : base(wrapped)
  {
    _wrapped = wrapped;
    IsAutoSubmit = false;
  }

  protected override void _ReciveEvent(AttackEvent @event)
  {

  }

  protected override void _SaveOperation(AttackEvent @event)
  {
    var fromPiece = PieceAdapter.GameManager.GetPiece(@event.fromFaction, @event.fromPiece);
    var targetPiece = PieceAdapter.GameManager.GetPiece(@event.targetFaction, @event.targetPiece);
    var instance = targetPiece.Instance.Origin as Node;
    var tag = instance.GetNode<Node>("Tag");
    Button combatButton;
    if ((combatButton = tag.GetNodeOrNull<Button>("Combat")) == null)
    {
      combatButton = new Button
      {
        Text = "Combat!",
        Name = "Combat"
      };
      combatButton.Pressed += () =>
      {
        var combatResult = CombatController.Instance.ProcessCombat(targetPiece.GetInstanceId());
        var op = ToOperation(@event, combatResult);
        GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, op);
        tag.RemoveChild(combatButton);
        combatButton.QueueFree();
      };
      targetPiece.GetNode<Node>("Instance").GetNode<Node>("Tag").AddChild(combatButton);
    }
  }

  protected Operation ToOperation(AttackEvent @event, CombatResult combatResult)
  {
    return new AttackOperation()
    {
      CombatResult = (int)combatResult,
      From = @event.fromPiece,
      FromFaction = @event.fromFaction,
      Target = @event.targetPiece,
      TargetFaction = @event.targetFaction,
      Type = (int)OperationType.ATTACK,
      CommentType = CommentType.GAME_DATA
    };
  }
}