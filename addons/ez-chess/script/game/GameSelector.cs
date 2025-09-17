using Godot;
using System;

public partial class GameSelector : Control
{
	private FileDialog _fileDialog;
	private PopupMenu _fileTab;
	private ItemList _itemList;
	private string _moduleTxt = "user://modules.txt";

	public override void _Ready()
	{
		_fileDialog = GetNode<FileDialog>("VBoxContainer/MenuBar/File/FileDialog");
		_fileTab = GetNode<PopupMenu>("VBoxContainer/MenuBar/File");
		_itemList = GetNode<ItemList>("VBoxContainer/ItemList");
		_fileTab.IdPressed += OpenModule;
		_fileDialog.FileSelected += OnFileSelected;
		_itemList.ItemActivated += OnItemActivated;
		FindModule();
	}

	private void FindModule()
	{
		if (FileAccess.FileExists(_moduleTxt))
		{
			var file = FileAccess.Open(_moduleTxt, FileAccess.ModeFlags.ReadWrite);
			while (file.GetPosition() < file.GetLength())
			{
				var module = file.GetLine();
				LoadModule(module);
			}
		}
	}

	private void OpenModule(long id)
	{
		if (id == 0)
		{
			_fileDialog.Show();
		}
	}

	private void SaveModule(string path)
	{
		if (FileAccess.FileExists(_moduleTxt))
		{
			var file = FileAccess.Open(_moduleTxt, FileAccess.ModeFlags.ReadWrite);
			file.SeekEnd();
			file.StoreLine(path);
		}
		else
		{
			var file = FileAccess.Open(_moduleTxt, FileAccess.ModeFlags.WriteRead);
			file.StoreLine(path);
		}
	}

	private void OnFileSelected(string path)
	{
		LoadModule(path);
		SaveModule(path);
	}

	private void LoadConfig(string path)
	{
		var baseDir = path.GetFile().GetBaseName();
		var configPath = $"res://{baseDir}/config.json";
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

				int index = _itemList.AddItem(name);
				_itemList.AddItem(version, null, false);
				_itemList.AddItem(description, null, false);
				_itemList.SetItemMetadata(index, jsonObject);
			}
		}

	}

	private void LoadModule(string path)
	{
		if (!ProjectSettings.LoadResourcePack(path))
		{
			GD.PrintErr("Error occurred while opening the module.");
		}
		else
		{
			LoadConfig(path);
		}
	}

	public void OnItemActivated(long id)
	{
		GameState.Instance.Config = _itemList.GetItemMetadata((int)id).AsGodotDictionary();
		var err = GetTree().ChangeSceneToFile("res://addons/ez-chess/scene/game_config.tscn");
		if (err != Error.Ok)
		{
			GD.PrintErr("Error occurred while changing the scene.");
		}
	}
}
