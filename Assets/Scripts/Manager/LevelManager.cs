using UnityEngine;
using System.IO;
using System;

public class LevelManager : IDisposable
{
	private readonly User user;
	public LevelData? CurrentLevelData { get; private set; }

	public LevelManager(User user)
	{
		this.user = user;
		user.Init();
		LoadCurrentLevelData();

		EventManager.OnWinClicked += OnLevelWin;
	}

	public void LoadCurrentLevelData()
	{
		int levelId = user.LevelId;
		CurrentLevelData = TryLoadLevelData(levelId);
	}

	public void OnLevelWin()
	{
		user.IncreaseLevel();
		LoadCurrentLevelData();
	}

	private LevelData? TryLoadLevelData(int levelId)
	{
        // the first level is intentionally loaded from resources
		var data = LoadLevelDataFromFile(LevelFileHelper.GetLevelFilePath(levelId, user.LevelDataPath));
		if (data != null)
			return data;

		data = LoadLevelDataFromResources(levelId);
		if (data != null)
			return data;

		return data;
	}

	private LevelData? LoadLevelDataFromFile(string filePath)
	{
		if (!File.Exists(filePath))
			return null;

			LevelData data = LevelBatchBinaryImporter.Deserialize(filePath);
		return data;
	}

	private LevelData? LoadLevelDataFromResources(int levelId)
	{
		string resourceName = $"{Constants.ResourcesLevelFile}{levelId}";
		var textAsset = Resources.Load<TextAsset>(resourceName);
		if (textAsset == null)
			return null;
		return Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>(textAsset.text);
	}

	public void Dispose()
	{
		EventManager.OnWinClicked -= OnLevelWin;
	}
}
