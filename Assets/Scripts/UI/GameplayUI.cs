using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour, IDisposable
{
    [Header("UI Elements")]
    [SerializeField] private Button winButton;
    [SerializeField] private TextMeshProUGUI levelIDText;
    [SerializeField] private TextMeshProUGUI boardText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    public void Init()
    {
        winButton?.onClick.AddListener(WinButtonClicked);

        EventManager.OnPlayClicked += HandlePlayClicked;
    }

    private void HandlePlayClicked()
    {
        winButton.gameObject.SetActive(true);
    }

    private void WinButtonClicked()
    {
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

        var sb = new StringBuilder();
        foreach (var row in levelData?.Board)
        {
            sb.AppendLine(string.Join(",", row));
        }
        boardText.text = sb.ToString();

        difficultyText.text = $"Difficulty: {levelData?.Difficulty}";
    }
    public void Dispose()
    {
        winButton?.onClick.RemoveListener(WinButtonClicked);
        EventManager.OnPlayClicked -= HandlePlayClicked;
    }
}
