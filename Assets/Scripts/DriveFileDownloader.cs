using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DriveFileDownloader : MonoBehaviour
{
    [Header("Drive File Links ScriptableObject")]
    public DriveFileLinks driveFileLinks;

    [Header("Dosyalar nereye kaydedilecek?")]
    public string saveFolder = "DownloadedLevels";

    public void DownloadFileByIndex(int index)
    {
        if (driveFileLinks == null || driveFileLinks.fileLinks == null)
        {
            Debug.LogError("DriveFileLinks asseti atanmadı.");
            return;
        }
        if (index < 0 || index >= driveFileLinks.fileLinks.Count)
        {
            Debug.LogError($"Geçersiz index: {index}");
            return;
        }
        string fileIdOrLink = driveFileLinks.fileLinks[index];
        string fileId = fileIdOrLink;
        if (fileIdOrLink.Contains("drive.google.com"))
        {
            var parts = fileIdOrLink.Split('/');
            int idx = System.Array.IndexOf(parts, "d");
            if (idx >= 0 && idx + 1 < parts.Length)
                fileId = parts[idx + 1];
        }
        string url = $"https://drive.google.com/uc?export=download&id={fileId}";
        StartCoroutine(DownloadAndSaveFile(url, fileId));
    }

    private IEnumerator DownloadAndSaveFile(string url, string fileId)
    {
        string persistentFolderPath = Path.Combine(Application.persistentDataPath, saveFolder);
        if (!Directory.Exists(persistentFolderPath))
            Directory.CreateDirectory(persistentFolderPath);

        string projectLevelsFolder = Path.Combine(Application.dataPath, "ProjectRoot/Levels");
        if (!Directory.Exists(projectLevelsFolder))
            Directory.CreateDirectory(projectLevelsFolder);

        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string fileName = fileId + ".json";
                string contentDisposition = uwr.GetResponseHeader("Content-Disposition");
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    var fileNameToken = "filename=";//dosya adını remote'dan al
                    int idx = contentDisposition.IndexOf(fileNameToken);
                    if (idx >= 0)
                    {
                        int startIdx = idx + fileNameToken.Length;
                        if (contentDisposition[startIdx] == '"') startIdx++;
                        int endIdx = contentDisposition.IndexOf('"', startIdx);
                        if (endIdx > startIdx)
                            fileName = contentDisposition.Substring(startIdx, endIdx - startIdx);
                        else
                        {
                            int semiIdx = contentDisposition.IndexOf(';', startIdx);
                            if (semiIdx > startIdx)
                                fileName = contentDisposition.Substring(startIdx, semiIdx - startIdx);
                            else
                                fileName = contentDisposition.Substring(startIdx);
                        }
                    }
                }

                string persistentFilePath = Path.Combine(persistentFolderPath, fileName);
                File.WriteAllBytes(persistentFilePath, uwr.downloadHandler.data);
                Debug.Log($"Dosya indirildi: {persistentFilePath}");

                string projectFilePath = Path.Combine(projectLevelsFolder, fileName);
                File.WriteAllBytes(projectFilePath, uwr.downloadHandler.data);
                Debug.Log($"Dosya kopyalandı: {projectFilePath}");
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            else
            {
                Debug.LogError($"Indirme hatası: {fileId} - {uwr.error}");
            }
        }
    }
}
