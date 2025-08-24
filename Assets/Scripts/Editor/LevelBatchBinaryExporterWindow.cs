using UnityEditor;
using UnityEngine;

// Level batch ve tekil level dosyalarını binary olarak export eden EditorWindow
public class LevelBatchBinaryExporterWindow : EditorWindow
{
    private int startLevel = Constants.StartLevel;
    private int endLevel = Constants.EndLevel;
    private string levelsFolder = Constants.LevelsFolder;
    private string outputFile = Constants.OutputFile;

    [MenuItem("Tools/Level Batch Binary Exporter")]
    public static void ShowWindow()
    {
        GetWindow<LevelBatchBinaryExporterWindow>("Level Batch Binary Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Batch Binary Exporter", EditorStyles.boldLabel);
        levelsFolder = EditorGUILayout.TextField("Levels Folder", levelsFolder);
        startLevel = EditorGUILayout.IntField("Start Level", startLevel);
        endLevel = EditorGUILayout.IntField("End Level", endLevel);
        outputFile = EditorGUILayout.TextField("Output File", outputFile);

        if (GUILayout.Button("Export Levels To Binary"))
        {
            LevelBatchBinaryExporter.ExportLevelsToBinary(levelsFolder, startLevel, endLevel, outputFile);
            AssetDatabase.Refresh();
        }
    }
}
