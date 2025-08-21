using VContainer;

public class LevelManager
{
	private readonly User _user;

	public LevelManager(User user)
	{
		_user = user;
	}

	public void Init()
	{
		_user.Init();
	}

	public void OnLevelWin()
	{
		_user.IncreaseLevel();
	}
}
