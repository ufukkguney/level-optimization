using System;

public static class EventManager
{
    public static event Action OnPlayClicked;
    public static event Action OnWinClicked;

    public static void InvokePlayClicked()
    {
        OnPlayClicked?.Invoke();
    }

    public static void InvokeWinClicked()
    {
        OnWinClicked?.Invoke();
    }
}
