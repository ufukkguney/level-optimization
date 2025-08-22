using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

public class DriveFileDownloader : MonoBehaviour
{
    [Header("Drive File Links ScriptableObject")]
    [Inject] private DriveFileLinks driveFileLinks;

    public async void DownloadFileByIndex(int index)
    {
        if (driveFileLinks == null || driveFileLinks.FileLinks == null)
        {
            Debug.LogError("DriveFileLinks asseti atanmadı.");
            return;
        }
        if (index < 0 || index >= driveFileLinks.FileLinks.Count)
        {
            Debug.LogError($"Geçersiz index: {index}");
            return;
        }
        string fileIdOrLink = driveFileLinks.FileLinks[index];
        string fileId = fileIdOrLink;
        if (fileIdOrLink.Contains("drive.google.com"))
        {
            var parts = fileIdOrLink.Split('/');
            int idx = System.Array.IndexOf(parts, "d");
            if (idx >= 0 && idx + 1 < parts.Length)
                fileId = parts[idx + 1];
        }
        string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
        if (!Directory.Exists(persistentFolderPath)) Directory.CreateDirectory(persistentFolderPath);
        string filePath = Path.Combine(persistentFolderPath, fileId);
        if (File.Exists(filePath))
        {
            Debug.Log($"Level dosyası zaten indirilmiş: {fileId}");
            return;
        }
        string url = $"https://drive.google.com/uc?export=download&id={fileId}";

        await DownloadAndSaveFileAsync(url, fileId);
    }

    private async Task DownloadAndSaveFileAsync(string url, string fileId)
    {
        string persistentFolderPath = Path.Combine(Application.persistentDataPath, Constants.SaveFolder);
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
            await Task.Run(() =>
            {
                string tempBatchFilePath = Path.Combine(persistentFolderPath, $"batch_{fileId}.bin");
                File.WriteAllBytes(tempBatchFilePath, allData);
                var levels = LevelBatchBinaryImporter.ImportLevelsFromBinary(tempBatchFilePath);
                Debug.Log($"{levels.Count} adet LevelData batch dosyasından parse edildi.");
                for (int i = 0; i < levels.Count; i++)
                {
                    string levelFileName = $"{Constants.LevelDataPath}{levels[i].Level}{Constants.LevelDataFileExtension}";
                    string levelFilePath = Path.Combine(persistentFolderPath, levelFileName);
                    using (var fs = new FileStream(levelFilePath, FileMode.Create, FileAccess.Write))
                    using (var bw = new BinaryWriter(fs))
                    {
                        LevelDataBinaryWriter.WriteLevelData(bw, levels[i]);
                    }
                    Debug.Log($"Level file path: {levelFilePath} exported. Level: {levels[i].Level}, LevelId: {levels[i].LevelId}, Difficulty: {levels[i].Difficulty}, GridSize: {levels[i].GridSize}, BoardRows: {levels[i].Board?.Length}");
                }
            });
        }
    }
}
