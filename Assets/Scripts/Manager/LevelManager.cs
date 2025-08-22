using VContainer;
using UnityEngine;
using System.IO;
using System;

public class LevelManager: IDisposable
{
	private readonly User _user;

	public LevelManager(User user)
	{
		_user = user;
	}

	public LevelData? CurrentLevelData { get; private set; }

    public void Dispose()
    {
        EventManager.OnWinClicked -= OnLevelWin;
    }

	public void Init()
	{
		_user.Init();
		LoadCurrentLevelData();
		EventManager.OnWinClicked += OnLevelWin;
	}

	public void LoadCurrentLevelData()
	{
		int levelId = _user.LevelId;
		string filePath = GetLevelFilePath(levelId);
		CurrentLevelData = LoadLevelDataFromFile(filePath);

		if (CurrentLevelData == null)
		{
			CurrentLevelData = LoadLevelDataFromResources(levelId);
			Debug.Log($"Loaded LevelData from Resources: {CurrentLevelData}");
		}
		Debug.Log($"Final LevelData: {CurrentLevelData}");
	}

	public void OnLevelWin()
	{
		_user.IncreaseLevel();
		LoadCurrentLevelData();
	}

	private string GetLevelFilePath(int levelId)
	{
		string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
		string checkFileName = $"{Constants.LevelDataPath}{levelId}{Constants.LevelDataFileExtension}";
		return Path.Combine(persistentFolderPath, checkFileName);
	}

	private LevelData? LoadLevelDataFromFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			Debug.Log($"Deserializing existing LevelData from {filePath}");
			return LevelBatchBinaryImporter.Deserialize(filePath);
		}
		return null;
	}

	private LevelData? LoadLevelDataFromResources(int levelId)
	{
		string resourceName = $"level_{levelId}";
		var textAsset = Resources.Load<TextAsset>(resourceName);
		if (textAsset != null)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>(textAsset.text);
		}
		return null;
	}
}
