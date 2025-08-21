using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;

public class DriveFileDownloader : MonoBehaviour
{
    [Header("Drive File Links ScriptableObject")]
    [Inject] private DriveFileLinks driveFileLinks;

    private const string SaveFolder = "DownloadedLevels";

    public void DownloadFileByIndex(int index)
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
        string url = $"https://drive.google.com/uc?export=download&id={fileId}";
        StartCoroutine(DownloadAndSaveFile(url, fileId));
    }

    private IEnumerator DownloadAndSaveFile(string url, string fileId)
    {
        string persistentFolderPath = Path.Combine(Application.persistentDataPath, SaveFolder);
        if (!Directory.Exists(persistentFolderPath)) Directory.CreateDirectory(persistentFolderPath);

        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Indirme hatası: {fileId} - {uwr.error}");
                yield break;
            }
            // string fileName = GetFileNameFromHeaderOrDefault(uwr, fileId);
            File.WriteAllBytes(Path.Combine(persistentFolderPath, fileId), uwr.downloadHandler.data);
            Debug.Log($"Dosya indirildi ve kopyalandı: {fileId}");

            
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }

    // private string GetFileNameFromHeaderOrDefault(UnityWebRequest uwr, string fileId)
    // {
    //     var sb = new System.Text.StringBuilder();
    //     sb.Append(fileId).Append(".json");
    //     string fileName = sb.ToString();
    //     string contentDisposition = uwr.GetResponseHeader("Content-Disposition");
    //     if (!string.IsNullOrEmpty(contentDisposition))
    //     {
    //         var fileNameToken = "filename=";
    //         int idx = contentDisposition.IndexOf(fileNameToken);
    //         if (idx >= 0)
    //         {
    //             int startIdx = idx + fileNameToken.Length;
    //             if (contentDisposition[startIdx] == '"') startIdx++;
    //             int endIdx = contentDisposition.IndexOf('"', startIdx);
    //             sb.Clear();
    //             if (endIdx > startIdx)
    //                 sb.Append(contentDisposition, startIdx, endIdx - startIdx);
    //             else
    //             {
    //                 int semiIdx = contentDisposition.IndexOf(';', startIdx);
    //                 if (semiIdx > startIdx)
    //                     sb.Append(contentDisposition, startIdx, semiIdx - startIdx);
    //                 else
    //                     sb.Append(contentDisposition.Substring(startIdx));
    //             }
    //             fileName = sb.ToString();
    //         }
    //     }
    //     return fileName;
    // }
}
