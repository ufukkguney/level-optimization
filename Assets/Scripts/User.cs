using UnityEngine;

public class User
{
    public int LevelId => Mathf.Max(_levelId, 1);
    private int _levelId;

    public void Init()
    {
        _levelId = PlayerPrefs.GetInt(Constants.LevelIdPrefKey, Constants.DefaultLevelId);
    }

    public void SaveLevelId()
    {
        PlayerPrefs.SetInt(Constants.LevelIdPrefKey, _levelId);
        PlayerPrefs.Save();
    }

    public void IncreaseLevel()
    {
        _levelId++;
        SaveLevelId();
    }
}
