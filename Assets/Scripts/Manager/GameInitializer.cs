using System;
using VContainer;
using VContainer.Unity;

public class GameInitializer : IStartable,IDisposable
{
    [Inject] private LevelFileManager _levelFileManager;
    [Inject] private LevelManager _levelManager;
    [Inject] private Gameplay _gameplay;
    [Inject] private HomeScreen _homeScreen ;

    public void Start()
    {
        _levelFileManager.CheckAndDownloadLevelBatch();
        _levelManager.Init();
        _homeScreen?.Init(_levelManager);
        _gameplay.Init();

        EventManager.OnPlayClicked += HandleHomeScreenPlayClicked;
    }

    public void Dispose()
    {
        EventManager.OnPlayClicked -= HandleHomeScreenPlayClicked;
    }

    private void HandleHomeScreenPlayClicked()
    {
        _gameplay.StartGameplay();
    }
    
}
