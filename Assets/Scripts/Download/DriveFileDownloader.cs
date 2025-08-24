using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

public class DriveFileDownloader
{
    [Inject] private DriveFileLinks driveFileLinks;

    public async void DownloadFileByIndex(int index, Action<byte[], string> processBatchFile)
    {
        if (!IsValidFileLinks()) return;
        if (!IsValidIndex(index)) return;

        string fileId = ExtractFileId(driveFileLinks.FileLinks[index]);
        string persistentFolderPath = GetOrCreatePersistentFolder();
        string filePath = Path.Combine(persistentFolderPath, fileId);

        if (File.Exists(filePath))
        {
            Debug.Log($"Level file already downloaded: {fileId}");
            return;
        }

        string url = BuildGoogleDriveDownloadUrl(fileId);
        await DownloadAndSaveFileAsync(url, fileId, persistentFolderPath, processBatchFile);
    }


    private async Task DownloadAndSaveFileAsync(string url, string fileId, string persistentFolderPath, Action<byte[], string> processBatchFile)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            var operation = uwr.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Download error: {fileId} - {uwr.error}");
                return;
            }

            byte[] allData = uwr.downloadHandler.data;
            // The largest incoming data parse operation using a thread
            await Task.Run(() => processBatchFile(allData, persistentFolderPath));
        }
    }
    
    private bool IsValidFileLinks()
    {
        if (driveFileLinks == null || driveFileLinks.FileLinks == null)
        {
            Debug.LogError("DriveFileLinks asset is not assigned.");
            return false;
        }
        return true;
    }

    private bool IsValidIndex(int index)
    {
        if (index < 0 || index >= driveFileLinks.FileLinks.Count)
        {
            Debug.LogError($"Invalid index: {index}");
            return false;
        }
        return true;
    }

    private string ExtractFileId(string fileIdOrLink)
    {
        if (fileIdOrLink.Contains("drive.google.com"))
        {
            var parts = fileIdOrLink.Split('/');
            int idx = System.Array.IndexOf(parts, "d");
            if (idx >= 0 && idx + 1 < parts.Length)
                return parts[idx + 1];
        }
        return fileIdOrLink;
    }

    private string GetOrCreatePersistentFolder()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    private string BuildGoogleDriveDownloadUrl(string fileId)
    {
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }
}
