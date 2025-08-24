using VContainer;
using UnityEngine;
using System.IO;
using System;

public class LevelManager : IDisposable
{
	[Inject] private readonly User _user;
	public LevelData? CurrentLevelData { get; private set; }

	public void Init()
	{
		_user.Init();
		LoadCurrentLevelData();

		EventManager.OnWinClicked += OnLevelWin;
	}

	public void LoadCurrentLevelData()
	{
		int levelId = _user.LevelId;
		Debug.Log($"Loading LevelData for LevelId: {levelId}");
		CurrentLevelData = TryLoadLevelData(levelId);
		Debug.Log($"Final LevelData: {CurrentLevelData}");
	}

	private LevelData? TryLoadLevelData(int levelId)
	{
		var data = LoadLevelDataFromFile(GetLevelFilePath(levelId));
		if (data != null)
			return data;
		data = LoadLevelDataFromResources(levelId);
		if (data != null)
			Debug.Log($"Loaded LevelData from Resources: {data}");
		return data;
	}

	public void OnLevelWin()
	{
		_user.IncreaseLevel();

		LoadCurrentLevelData();
	}
	private string GetLevelFilePath(int levelId)
	{
		string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
		string fileName = $"{Constants.LevelDataPath}{levelId}{Constants.LevelDataFileExtension}";
		return Path.Combine(persistentFolderPath, fileName);
	}

	private LevelData? LoadLevelDataFromFile(string filePath)
	{
		if (!File.Exists(filePath))
			return null;
		Debug.Log($"Deserializing existing LevelData from {filePath}");
		return LevelBatchBinaryImporter.Deserialize(filePath);
	}

	private LevelData? LoadLevelDataFromResources(int levelId)
	{
		string resourceName = $"level_{levelId}";
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
