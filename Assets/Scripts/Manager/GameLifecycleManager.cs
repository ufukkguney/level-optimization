using System;
using VContainer;
using VContainer.Unity;

public class GameLifecycleManager : IStartable,IDisposable
{
    [Inject] private LevelFileManager levelFileManager;
    [Inject] private Gameplay gameplay;
    [Inject] private HomeScreen homeScreen ;

    public void Start()
    {
        levelFileManager?.Init();
        homeScreen?.Init();
        gameplay?.Init();

        EventManager.OnPlayClicked += HandleHomeScreenPlayClicked;
    }
    
    private void HandleHomeScreenPlayClicked()
    {
        gameplay.StartGameplay();
    }

    public void Dispose()
    {
        EventManager.OnPlayClicked -= HandleHomeScreenPlayClicked;
    }
}
