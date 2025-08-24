using System;
using VContainer;

public class Gameplay : IDisposable
{
    [Inject] private LevelManager levelManager;
    [Inject] private GameplayUI gameplayUI;

    public LevelData? CurrentLevelData => levelManager.CurrentLevelData;

    public void Init()
    {
        gameplayUI?.Init();

        EventManager.OnPlayClicked += StartGameplay;
    }

    public void StartGameplay()
    {
        gameplayUI?.UpdateLevelInfo(CurrentLevelData);
    }

    public void Dispose()
    {
        EventManager.OnPlayClicked -= StartGameplay;
    }
}
