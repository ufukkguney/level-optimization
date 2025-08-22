using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour, IDisposable
{
    [Header("UI Elements")]
    [SerializeField] public Button winButton;
    [SerializeField] private TextMeshProUGUI levelIDText;
    [SerializeField] private TextMeshProUGUI boardText;
    [SerializeField] private TextMeshProUGUI difficultyText;


    public void Init()
    {
        winButton?.onClick.AddListener(WinButtonClicked);

        EventManager.OnPlayClicked += HandlePlayClicked;
    }

    public void Dispose()
    {
        winButton?.onClick.RemoveListener(WinButtonClicked);
        EventManager.OnPlayClicked -= HandlePlayClicked;
    }

    private void HandlePlayClicked()
    {
        winButton.gameObject.SetActive(true);
    }

    private void WinButtonClicked()
    {
        Debug.Log("Win button clicked");
        EventManager.InvokeWinClicked();
        winButton.gameObject.SetActive(false);

        levelIDText.text = "";
        boardText.text = "";
        difficultyText.text = "";
    }

    public void UpdateLevelInfo(LevelData? levelData)
    {
        if (levelData == null) return;
        levelIDText.text = $"Level ID: {levelData?.LevelId}";

        boardText.text = string.Join("\n", Array.ConvertAll(levelData?.Board, row => string.Join(",", row)));
        difficultyText.text = $"Difficulty: {levelData?.Difficulty}";
    }
}
