using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HomeScreen : MonoBehaviour, IDisposable
{
    [SerializeField] public Button playButton;
    private LevelManager _levelManager;

    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
        Debug.Log($"Initializing HomeScreen with LevelData: {_levelManager.CurrentLevelData?.LevelId ?? "null"}");
        playButton?.onClick.AddListener(PlayButtonClicked);
        SetPlayButtonColor(_levelManager.CurrentLevelData?.Difficulty);

        EventManager.OnWinClicked += HandleWinClicked;
    }

    private void HandleWinClicked()
    {
        SetPlayButtonColor(_levelManager.CurrentLevelData?.Difficulty);
        playButton.gameObject.SetActive(true);
    }

    private void PlayButtonClicked()
    {
        Debug.Log("Play button clicked");
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
