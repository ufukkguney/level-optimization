using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

// EditorWindow that imports and deserializes level batch files as binary
public class LevelBatchBinaryImporterWindow : EditorWindow
{
    private string batchFilePath = Constants.BinaryFile;
    private string singleLevelFilePath = Constants.SingleLevelFile;
    private Vector2 batchScrollPos;
    private Vector2 singleScrollPos;
    private List<LevelData> batchLevels;
    private LevelData singleLevel = default;

    [MenuItem("Tools/Level Batch Binary Importer")]
    public static void ShowWindow()
    {
        GetWindow<LevelBatchBinaryImporterWindow>("Level Batch Binary Importer");
    }

    private void OnGUI()
    {
        DrawBatchImportSection();
        GUILayout.Space(10);
        DrawSingleLevelSection();
    }

    private void DrawBatchImportSection()
    {
        GUILayout.Label("Batch Level Import", EditorStyles.boldLabel);
        batchFilePath = EditorGUILayout.TextField("Batch Binary File", batchFilePath);

        if (GUILayout.Button("Import Levels From Batch File"))
        {
            if (!string.IsNullOrEmpty(batchFilePath) && File.Exists(batchFilePath))
            {
                batchLevels = LevelBatchBinaryImporter.ImportLevelsFromLevelBatch(batchFilePath);
            }
            else
            {
                batchLevels = null;
                Debug.LogWarning("Batch file path is invalid or file not found.");
            }
        }

        if (batchLevels != null)
        {
            GUILayout.Label($"Imported Levels: {batchLevels.Count}");
            batchScrollPos = EditorGUILayout.BeginScrollView(batchScrollPos, GUILayout.Height(200));
            foreach (var level in batchLevels)
            {
                GUILayout.Label($"Level {level.Level} | Id: {level.LevelId} | Difficulty: {level.Difficulty} | GridSize: {level.GridSize}");
                if (level.Board != null)
                {
                    for (int r = 0; r < level.Board.Length; r++)
                    {
                        GUILayout.Label(string.Join(",", level.Board[r]));
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawSingleLevelSection()
    {
        GUILayout.Label("Single Level Deserialize", EditorStyles.boldLabel);
        singleLevelFilePath = EditorGUILayout.TextField("Single Level File", singleLevelFilePath);

        if (GUILayout.Button("Deserialize Single Level"))
        {
            if (!string.IsNullOrEmpty(singleLevelFilePath) && File.Exists(singleLevelFilePath))
            {
                singleLevel = LevelBatchBinaryImporter.Deserialize(singleLevelFilePath);
            }
            else
            {
                singleLevel = default;
                Debug.LogWarning("Single level file path is invalid or file not found.");
            }
        }

        if (singleLevel.Board != null)
        {
            GUILayout.Label($"Deserialized Level: {singleLevel.Level} | Id: {singleLevel.LevelId} | Difficulty: {singleLevel.Difficulty} | GridSize: {singleLevel.GridSize}");
            singleScrollPos = EditorGUILayout.BeginScrollView(singleScrollPos, GUILayout.Height(200));
            for (int r = 0; r < singleLevel.Board.Length; r++)
            {
                GUILayout.Label(string.Join(",", singleLevel.Board[r]));
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
