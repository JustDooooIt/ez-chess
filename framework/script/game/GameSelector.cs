using Godot;
using System;

public partial class GameSelector : Control
{
  private FileDialog _fileDialog;
  private PopupMenu _fileTab;
  private ItemList _itemList;

  public override void _Ready()
  {
    _fileDialog = GetNode<FileDialog>("VBoxContainer/MenuBar/File/FileDialog");
    _fileTab = GetNode<PopupMenu>("VBoxContainer/MenuBar/File");
    _itemList = GetNode<ItemList>("VBoxContainer/ItemList");
    _fileTab.IdPressed += OpenModule;
    _fileDialog.FileSelected += OnFileSelected;
    _itemList.ItemActivated += OnItemActivated;
  }

  private void OpenModule(long id)
  {
    if (id == 0)
    {
      _fileDialog.Show();
    }
  }

  private void OnFileSelected(string path)
  {
    if (!ProjectSettings.LoadResourcePack(path))
    {
      GD.PrintErr("Error occurred while opening the module.");
    }
    else
    {
      var baseDir = path.GetFile().GetBaseName();
      var configPath = $"res://{baseDir}/config/config.json";
      if (FileAccess.FileExists(configPath))
      {
        var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Read);
        if (FileAccess.GetOpenError() != Error.Ok)
        {
          GD.PrintErr("Error occurred while getting the config.");
        }
        else
        {
          var jsonObject = Json.ParseString(file.GetAsText()).AsGodotDictionary();
          var name = jsonObject["name"].AsString();
          var version = jsonObject["version"].AsString();
          var description = jsonObject["description"].AsString();

          var index = _itemList.AddItem(name);
          _itemList.AddItem(version, null, false);
          _itemList.AddItem(description, null, false);
          _itemList.SetItemMetadata(index, baseDir);
        }
      }
    }
  }

  public void OnItemActivated(long id)
  {
    var basePath = _itemList.GetItemMetadata((int)id).AsString();
    var err = GetTree().ChangeSceneToFile(basePath + "/config/game_config.tscn");
    if (err != Error.Ok)
    {
      GD.PrintErr("Error occurred while changing the scene.");
    }
  }
}
