using VContainer;
using UnityEngine;

public class Gameplay
{
    [Inject] private LevelManager _levelManager;
    [Inject] private GameplayUI gameplayUI;

    public LevelData? CurrentLevelData => _levelManager.CurrentLevelData;

    public void Init()
    {
        gameplayUI?.Init();
    }

    public void StartGameplay()
    {
        Debug.Log($"Gameplay başladı. LevelData: {CurrentLevelData?.LevelId ?? "null"}");
        gameplayUI?.UpdateLevelInfo(CurrentLevelData);
    }
}
