using UnityEngine;
using VContainer;
using System.IO;
using System;

public class LevelFileManager : IDisposable
{
    private readonly User _user;
    [Inject] private DriveFileDownloader _downloader;

    [Inject]
    public LevelFileManager(User user)
    {
        _user = user;
        EventManager.OnWinClicked += CheckAndDownloadLevelBatch;
        EventManager.OnWinClicked += DeletePreviousLevelFile;

    }
    public void CheckAndDownloadLevelBatch()
    {
        int levelId = _user.LevelId;
        string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
        int checkLevel = levelId + Constants.LevelCheckOffset;
        string checkFileName = $"{Constants.LevelDataPath}{checkLevel}{Constants.LevelDataFileExtension}";
        string checkFilePath = Path.Combine(persistentFolderPath, checkFileName);

        if (!File.Exists(checkFilePath))
        {
            int batchIndex = checkLevel / Constants.LevelBatchSize;
            _downloader.DownloadFileByIndex(batchIndex);
            Debug.Log($"Batch {batchIndex} (Level {checkLevel}) indiriliyor...");
        }
        else
        {
            Debug.Log($"Level {checkLevel} dosyası zaten mevcut.");
        }
    }
    public void DeletePreviousLevelFile()
    {
        int previousLevel = _user.LevelId - 1;
        if (previousLevel < 0) return;

        string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
        string fileName = $"{Constants.LevelDataPath}{previousLevel}{Constants.LevelDataFileExtension}";
        string filePath = Path.Combine(persistentFolderPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Level {previousLevel} dosyası silindi: {filePath}");
        }
    }
    
    public void Dispose()
    {
        EventManager.OnWinClicked -= CheckAndDownloadLevelBatch;
        EventManager.OnWinClicked -= DeletePreviousLevelFile;
    }
}
