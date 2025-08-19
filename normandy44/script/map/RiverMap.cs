#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

[Tool]
public partial class RiverMap : TileMapLayer
{
  // 你的 TileSet 里“河流”Atlas 的 Source Id（选中 AtlasSource 看 Inspector 顶部的 ID）
  [Export]
  public int RiverSourceId = 0;
  private Dictionary<int, Vector2I> _hexMask = [];
  private bool _wasPressedLastFrame = false;

  public override void _Ready()
  {
    _hexMask = LoadHexMapping("res://normandy44/resource/tile/hex_spritesheet_64_param_mapping.json");
  }

  public static Dictionary<int, Vector2I> LoadHexMapping(string jsonPath)
  {
    var mapping = new Dictionary<int, Vector2I>();

    // 打开文件
    using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
    if (file == null)
    {
      GD.PushError($"无法打开文件: {jsonPath}");
      return mapping;
    }

    string text = file.GetAsText();
    file.Close();

    // 解析 JSON
    JsonDocument doc;
    try
    {
      doc = JsonDocument.Parse(text);
    }
    catch (Exception e)
    {
      GD.PushError($"JSON 解析失败: {e.Message}");
      return mapping;
    }

    if (!doc.RootElement.TryGetProperty("mapping_dict", out var mappingDict))
    {
      GD.PushError("JSON 中没有 mapping_dict");
      return mapping;
    }

    foreach (var kvp in mappingDict.EnumerateObject())
    {
      int maskInt = int.Parse(kvp.Name);
      var cell = kvp.Value;

      int x = cell.GetProperty("x").GetInt32();
      int y = cell.GetProperty("y").GetInt32();

      mapping[maskInt] = new Vector2I(x, y);
    }

    return mapping;
  }

  public override void _Process(double delta)
  {
    // 轮询鼠标左键“刚刚按下”的瞬间（自己做沿帧检测，避免必须配置 InputMap）
    bool pressed = Input.IsMouseButtonPressed(MouseButton.Left);
    if (pressed && !_wasPressedLastFrame)
    {
      HandleClick();
    }
    _wasPressedLastFrame = pressed;
  }

  private void HandleClick()
  {
    // 1) 获取鼠标在 TileMap 本地坐标系中的位置
    Vector2 localMouse = GetLocalMousePosition();

    // 2) 转为网格坐标（cell 坐标）
    Vector2I cell = LocalToMap(localMouse);

    // （可选）如果你只关心有瓦片的 cell，可检测是否为空：
    // var tileData = GetCellTileData(0, cell); // 0 表默认层（或用 LayerId）
    // if (tileData == null) return;

    // 3) 得到该 cell 的几何中心位置
    Vector2 cellCenter = MapToLocal(cell);

    // 4) 计算鼠标相对 cell 中心的向量
    Vector2 rel = localMouse - cellCenter;

    // 5) 判断更靠近哪条边
    string edge;
    if (Mathf.Abs(rel.X) > Mathf.Abs(rel.Y))
    {
      edge = rel.X > 0 ? "右边" : "左边";
    }
    else
    {
      edge = rel.Y > 0 ? "下边" : "上边";
    }

    GD.Print($"点击的 cell: {cell}, 边: {edge}");
  }
}
