using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LevelBatchBinaryImporterWindow : EditorWindow
{
    private string binaryFile = Constants.BinaryFile;
    private string singleLevelFile = Constants.SingleLevelFile;
    private Vector2 scrollPos;
    private List<LevelData> importedLevels;
    private LevelData deserializedLevel = default;

    [MenuItem("Tools/Level Batch Binary Importer")]
    public static void ShowWindow()
    {
        GetWindow<LevelBatchBinaryImporterWindow>("Level Batch Binary Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Batch Binary Importer", EditorStyles.boldLabel);
        binaryFile = EditorGUILayout.TextField("Batch Binary File", binaryFile);

        if (GUILayout.Button("Import Levels From Binary"))
        {
            importedLevels = LevelBatchBinaryImporter.ImportLevelsFromBinary(binaryFile);
        }

        GUILayout.Space(10);
        GUILayout.Label("Single Level Binary Deserialize", EditorStyles.boldLabel);

        singleLevelFile = EditorGUILayout.TextField("Single Level File", singleLevelFile);

        if (GUILayout.Button("Deserialize Single Level"))
        {
            Debug.Log($"Deserializing single level from: {singleLevelFile}");
            if (!string.IsNullOrEmpty(singleLevelFile) && File.Exists(singleLevelFile))
            {
                deserializedLevel = LevelBatchBinaryImporter.Deserialize(singleLevelFile);
            }
            else
            {
                deserializedLevel = default;
            }
        }

        if (importedLevels != null)
        {
            GUILayout.Label($"Imported Levels: {importedLevels.Count}");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            for (int i = 0; i < importedLevels.Count; i++)
            {
                var level = importedLevels[i];
                GUILayout.Label($"Level {level.Level} | Id: {level.LevelId} | Difficulty: {level.Difficulty} | GridSize: {level.GridSize}");
                for (int r = 0; r < level.Board.Length; r++)
                {
                    GUILayout.Label(string.Join(",", level.Board[r]));
                }
            }
            EditorGUILayout.EndScrollView();
        }

        if (deserializedLevel.Board != null)
        {
            GUILayout.Label($"Deserialized Level: {deserializedLevel.Level} | Id: {deserializedLevel.LevelId} | Difficulty: {deserializedLevel.Difficulty} | GridSize: {deserializedLevel.GridSize}");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            for (int r = 0; r < deserializedLevel.Board.Length; r++)
            {
                GUILayout.Label(string.Join(",", deserializedLevel.Board[r]));
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
