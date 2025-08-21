using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private DriveFileLinks driveFileLinks;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<User>(Lifetime.Singleton);
        builder.Register<LevelManager>(Lifetime.Singleton);

        builder.Register<DriveFileDownloader>(Lifetime.Singleton);
        builder.RegisterInstance(driveFileLinks);
    }
}
