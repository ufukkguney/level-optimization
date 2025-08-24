using System.IO;
using UnityEngine;

public class User
{
    public int LevelId => levelId;
    private int levelId;
    public string LevelDataPath;

    public void Init()
    {
        levelId = PlayerPrefs.GetInt(Constants.LevelIdPrefKey, Constants.DefaultLevelId);
        levelId = Mathf.Max(levelId, 1);
        LevelDataPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
    }

    public void SaveLevelId()
    {
        PlayerPrefs.SetInt(Constants.LevelIdPrefKey, levelId);
        PlayerPrefs.Save();
    }

    public void IncreaseLevel()
    {
        levelId++;
        levelId = Mathf.Max(levelId, 1);
        SaveLevelId();
    }
}
