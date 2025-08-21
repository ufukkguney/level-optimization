using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelBatchBinaryImporterWindow : EditorWindow
{
    private string binaryFile = "Assets/ProjectRoot/Binaries/levels_1_25.bin";
    private Vector2 scrollPos;
    private List<LevelData> importedLevels;

    [MenuItem("Tools/Level Batch Binary Importer")]
    public static void ShowWindow()
    {
        GetWindow<LevelBatchBinaryImporterWindow>("Level Batch Binary Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Batch Binary Importer", EditorStyles.boldLabel);
        binaryFile = EditorGUILayout.TextField("Binary File", binaryFile);

        if (GUILayout.Button("Import Levels From Binary"))
        {
            importedLevels = LevelBatchBinaryImporter.ImportLevelsFromBinary(binaryFile);
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
    }
}
