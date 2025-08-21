using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class LevelBatchBinaryExporter
{
    public static void ExportLevelsToBinary(string levelsFolder, int startLevel, int endLevel, string outputFilePath)
    {
        var levelDatas = new List<LevelData>();
        var files = Directory.GetFiles(levelsFolder, "level*_updated");
        Debug.Log(files.Length + " adet level dosyası bulundu.");
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('_');
            if (parts.Length < 2) continue;
            if (!int.TryParse(parts[1], out int levelNum)) continue;
            if (levelNum < startLevel || levelNum > endLevel) continue;
            string json = File.ReadAllText(file);
            Debug.Log(json);
            LevelData data = JsonConvert.DeserializeObject<LevelData>(json);
            Debug.Log($"Level {data.Level} | Id: {data.LevelId} | Difficulty: {data.Difficulty} | GridSize: {data.Board[0]}");
            levelDatas.Add(data);
        }
        using (var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(levelDatas.Count);
            foreach (var level in levelDatas)
            {
                bw.Write(level.Level);
                bw.Write(level.LevelId ?? "");
                bw.Write(level.Difficulty ?? "");
                bw.Write(level.GridSize);
                // board
                bw.Write(level.Board.Length);
                foreach (var row in level.Board)
                {
                    bw.Write(row.Length);
                    foreach (var cell in row)
                        bw.Write(cell ?? "");
                }
            }
        }
        Debug.Log($"{levelDatas.Count} adet level {outputFilePath} dosyasına binary olarak export edildi.");
    }
}
