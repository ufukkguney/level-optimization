using UnityEngine;

public class User
{
    public int LevelId => Mathf.Max(_levelId, 1);
    private int _levelId;

    public void Init()
    {
        _levelId = PlayerPrefs.GetInt(Constants.LevelIdPrefKey, Constants.DefaultLevelId);
        _levelId = Mathf.Max(_levelId, 1);
    }

    public void SaveLevelId()
    {
        PlayerPrefs.SetInt(Constants.LevelIdPrefKey, _levelId);
        PlayerPrefs.Save();
    }

    public void IncreaseLevel()
    {
        _levelId++;
        _levelId = Mathf.Max(_levelId, 1);
        SaveLevelId();
        Debug.Log($"Level increased to {_levelId}");
    }
}
