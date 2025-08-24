using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HomeScreen : MonoBehaviour, IDisposable
{
    [SerializeField] public Button playButton;
    [Inject] private LevelManager levelManager;

    public void Init()
    {
        playButton?.onClick.AddListener(PlayButtonClicked);
        SetPlayButtonColor(levelManager.CurrentLevelData?.Difficulty);

        EventManager.OnWinClicked += HandleWinClicked;
    }

    private void HandleWinClicked()
    {
        SetPlayButtonColor(levelManager.CurrentLevelData?.Difficulty);
        playButton.gameObject.SetActive(true);
    }

    private void PlayButtonClicked()
    {
        EventManager.InvokePlayClicked();
        playButton.gameObject.SetActive(false);
    }

    private void SetPlayButtonColor(string difficulty)
    {
        if (playButton == null) return;
        Color color = Constants.DefaultButtonColor;
        switch (difficulty)
        {
            case Constants.DifficultyEasy:
                color = Constants.EasyColor;
                break;
            case Constants.DifficultyMedium:
                color = Constants.MediumColor;
                break;
            case Constants.DifficultyHard:
                color = Constants.HardColor;
                break;
        }
        playButton.image.color = color;
    }
    
    public void Dispose()
    {
        playButton?.onClick.RemoveListener(PlayButtonClicked);
        
        EventManager.OnWinClicked -= HandleWinClicked;
    }
}
