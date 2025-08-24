using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

public class DriveFileDownloader
{
    [Inject] private DriveFileLinks driveFileLinks;

    public async void DownloadFileByIndex(int index)
    {
        if (!IsValidFileLinks()) return;
        if (!IsValidIndex(index)) return;

        string fileId = ExtractFileId(driveFileLinks.FileLinks[index]);
        string persistentFolderPath = GetOrCreatePersistentFolder();
        string filePath = Path.Combine(persistentFolderPath, fileId);

        if (File.Exists(filePath))
        {
            Debug.Log($"Level dosyası zaten indirilmiş: {fileId}");
            return;
        }

        string url = BuildGoogleDriveDownloadUrl(fileId);
        await DownloadAndSaveFileAsync(url, fileId, persistentFolderPath);
    }


    private async Task DownloadAndSaveFileAsync(string url, string fileId, string persistentFolderPath)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            var operation = uwr.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Indirme hatası: {fileId} - {uwr.error}");
                return;
            }

            byte[] allData = uwr.downloadHandler.data;
            //25 adet olan level batch leri için en performanslı yöntem ayrı thread'de process olduğundan bu yöntem tercih edildi
            await Task.Run(() => ProcessBatchFile(allData, fileId, persistentFolderPath));
        }
    }

    private void ProcessBatchFile(byte[] allData, string fileId, string persistentFolderPath)
    {
        string tempBatchFilePath = Path.Combine(persistentFolderPath, $"batch_{fileId}.bin");
        //File.WriteAllBytes yerine buffer lı yazmak daha performanslı
        int bufferSize = Constants.BufferSize;
        using (var fs = new FileStream(tempBatchFilePath, FileMode.Create, FileAccess.Write))
        {
            int offset = 0;
            while (offset < allData.Length)
            {
                int bytesToWrite = Math.Min(bufferSize, allData.Length - offset);
                fs.Write(allData, offset, bytesToWrite);
                offset += bytesToWrite;
            }
        }

        var levels = LevelBatchBinaryImporter.ImportLevelsFromLevelBatch(tempBatchFilePath);
        //bir corruption olursa dosyaya yazmıyor
        Debug.Log($"{levels.Count} adet LevelData batch dosyasından parse edildi.");
        foreach (var level in levels)
        {
            string levelFileName = $"{Constants.LevelDataPath}{level.Level}{Constants.LevelDataFileExtension}";
            string levelFilePath = Path.Combine(persistentFolderPath, levelFileName);
            using (var fs = new FileStream(levelFilePath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                LevelDataBinaryWriter.WriteLevelData(bw, level);
            }
            Debug.Log($"Level file path: {levelFilePath} exported. Level: {level.Level}, LevelId: {level.LevelId}, Difficulty: {level.Difficulty}, GridSize: {level.GridSize}, BoardRows: {level.Board?.Length}");
        }
        // Batch dosyası parse edildikten sonra siliniyor
        if (File.Exists(tempBatchFilePath))
        {
            File.Delete(tempBatchFilePath);
            Debug.Log($"Temp batch dosyası silindi: {tempBatchFilePath}");
        }
    }
    
    private bool IsValidFileLinks()
    {
        if (driveFileLinks == null || driveFileLinks.FileLinks == null)
        {
            Debug.LogError("DriveFileLinks asseti atanmadı.");
            return false;
        }
        return true;
    }

    private bool IsValidIndex(int index)
    {
        if (index < 0 || index >= driveFileLinks.FileLinks.Count)
        {
            Debug.LogError($"Geçersiz index: {index}");
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
