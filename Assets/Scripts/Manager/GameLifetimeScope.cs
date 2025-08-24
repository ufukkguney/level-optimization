using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private DriveFileLinks driveFileLinks;
    [SerializeField] private HomeScreen homeScreen;
    [SerializeField] private GameplayUI gameplayUI;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameLifecycleManager>(Lifetime.Singleton);

        builder.Register<LevelManager>(Lifetime.Singleton);
        builder.Register<DriveFileDownloader>(Lifetime.Singleton);
        builder.Register<LevelFileManager>(Lifetime.Singleton);
        builder.Register<Gameplay>(Lifetime.Singleton);
        builder.Register<User>(Lifetime.Singleton);

        builder.RegisterInstance(driveFileLinks);
        builder.RegisterInstance(homeScreen);
        builder.RegisterInstance(gameplayUI);

    }
}
