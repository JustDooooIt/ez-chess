using Godot;

public partial class GeneralPiece : PieceAdapter
{
  private bool isInArea = false;
  public bool IsSelected { get; set; }

  public override void _Ready()
  {
    base._Ready();
    Instance.Area.InputEvent += OnAreaInput;
    Instance.Area.MouseEntered += () => { isInArea = true; };
    Instance.Area.MouseExited += () => { isInArea = false; };
  }

  public void Move(Vector2I from, Vector2I to)
  {
    if (IsSelected)
    {
      State.As<IMoveable>().Move(from, to);
    }
  }

  private void OnAreaInput(Node viewport, InputEvent @event, long shapeIndex)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      if (mouseButton.ButtonMask == MouseButtonMask.Left)
      {
        IsSelected = true;
      }
    }
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      // GD.Print();
      if (mouseButton.ButtonMask == MouseButtonMask.Left && !isInArea)
      {
        IsSelected = false;
      }
      if (mouseButton.ButtonMask == MouseButtonMask.Right && IsSelected)
      {
        var from = Instance.TerrainLayers.ToMapPosition(Instance.TerrainLayers.ToLocal(((Node2D)Instance.Origin).Position));
        var mousePosition = Instance.TerrainLayers.GetGlobalMousePosition();
        var localPosition = Instance.TerrainLayers.ToLocal(mousePosition);
        var to = Instance.TerrainLayers.ToMapPosition(localPosition);
        Move(from, to);
      }
    }
  }
}