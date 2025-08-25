using VContainer;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelFileManager : IDisposable
{
    [Inject] private readonly User user;
    [Inject] private readonly DriveFileDownloader downloader;
    
    public LevelFileManager()
    {
        EventManager.OnWinClicked += CheckAndDownloadLevelBatch;
        EventManager.OnWinClicked += DeletePreviousLevelFile;
    }
    public void Init()
    {
        CheckAndDownloadLevelBatch();
    }
    public void CheckAndDownloadLevelBatch()
    {
        int levelId = user.LevelId;
        int checkLevel = levelId + Constants.LevelCheckOffset;
        string checkFilePath = LevelFileHelper.GetLevelFilePath(checkLevel, user.LevelDataPath);

        if (!File.Exists(checkFilePath))
        {
            int batchIndex = checkLevel / Constants.LevelBatchSize;
            downloader.DownloadFileByIndex(batchIndex, ProcessBatchFile);
        }
    }
    private void ProcessBatchFile(byte[] allData, string persistentFolderPath)
    {
        // reading directly from memory is the most performant method when processing level batch
        List<LevelData> levels;
        using (var ms = new MemoryStream(allData))
        using (var br = new BinaryReader(ms))
        {
            levels = LevelBatchBinaryImporter.ImportLevelsFromStream(ms, br);
        }
        if (levels.Count == 0)
        {
            Debug.LogWarning("Downloaded level batch is corrupted or empty.");
            return;
        }
        else
        {
            foreach (var level in levels)
            {
                string levelFileName = $"{Constants.LevelDataPath}{level.Level}{Constants.LevelDataFileExtension}";
                string levelFilePath = Path.Combine(persistentFolderPath, levelFileName);
                using (var fs = new FileStream(levelFilePath, FileMode.Create, FileAccess.Write))
                using (var bw = new BinaryWriter(fs))
                {
                    LevelFileHelper.WriteLevelData(bw, level);
                }
            }
            DeleteAllPreviousLevelFiles();
        }
    }

    public void DeletePreviousLevelFile()
    {
        int previousLevel = user.LevelId - 1;
        if (previousLevel < 0) return;

        string filePath = LevelFileHelper.GetLevelFilePath(previousLevel, user.LevelDataPath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private void DeleteAllPreviousLevelFiles()//this is for better memory management when edge cases occur
    {
        int currentLevel = user.LevelId;
        for (int level = 0; level < currentLevel; level++)
        {
            string filePath = LevelFileHelper.GetLevelFilePath(level, user.LevelDataPath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    public void Dispose()
    {
        EventManager.OnWinClicked -= CheckAndDownloadLevelBatch;
        EventManager.OnWinClicked -= DeletePreviousLevelFile;
    }
}
