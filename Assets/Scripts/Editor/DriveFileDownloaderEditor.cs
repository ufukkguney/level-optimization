using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DriveFileDownloader))]
public class DriveFileDownloaderEditor : Editor
{
    int downloadIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DriveFileDownloader downloader = (DriveFileDownloader)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tek Dosya İndir (Index)", EditorStyles.boldLabel);
        downloadIndex = EditorGUILayout.IntField("Index", downloadIndex);

        if (GUILayout.Button($"Dosyayı İndir (Index: {downloadIndex})"))
        {
            downloader.DownloadFileByIndex(downloadIndex);
        }
    }
}
