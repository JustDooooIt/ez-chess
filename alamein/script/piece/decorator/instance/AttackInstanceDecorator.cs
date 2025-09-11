using Godot;

public partial class AttackInstanceDecorator : PieceInstanceDecorator<AttackEvent>, IAttackable
{
  private IPieceInstance _wrapped;

  public AttackInstanceDecorator(IPieceInstance wrapped) : base(wrapped)
  {
    _wrapped = wrapped;
  }

  protected override void DoReciveEvent(AttackEvent @event)
  {

  }

  protected override void SaveOperation(AttackEvent @event)
  {
    var fromPiece = PieceAdapter.GameManager.GetPiece(@event.fromFaction, @event.fromPiece);
    var targetPiece = PieceAdapter.GameManager.GetPiece(@event.targetFaction, @event.targetPiece);
    var instance = targetPiece.Instance.Origin as Node;
    var tag = instance.GetNode<Node>("Tag");
    Button combatButton = null;
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
        SaveOperation(fromPiece.Faction, fromPiece.Name, targetPiece.Faction, targetPiece.Name, combatResult);
        tag.RemoveChild(combatButton);
        combatButton.QueueFree();
      };
      targetPiece.GetNode<Node>("Instance").GetNode<Node>("Tag").AddChild(combatButton);
    }
  }

  private void SaveOperation(int fromFaction, string fromPiece, int targetFaction, string targetPiece, CombatResult combatResult)
  {

    AttackOperation operation = new()
    {
      CombatResult = (int)combatResult,
      From = fromPiece,
      FromFaction = fromFaction,
      Target = targetPiece,
      TargetFaction = targetFaction,
      Type = (int)OperationType.ATTACK,
      CommentType = CommentType.GAME_DATA
    };
    GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, operation);
  }
}