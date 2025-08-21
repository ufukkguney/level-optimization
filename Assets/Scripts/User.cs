using UnityEngine;

public struct User
{
    private const string LevelIdPrefKey = "User_LevelId";
    public int LevelId { get; private set; }

    public void Init()
    {
        LevelId = PlayerPrefs.GetInt(LevelIdPrefKey, 0);
    }

    public void SaveLevelId()
    {
        PlayerPrefs.SetInt(LevelIdPrefKey, LevelId);
        PlayerPrefs.Save();
    }

    public void IncreaseLevel()
    {
        LevelId++;
        SaveLevelId();
    }
}
