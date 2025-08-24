using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// LevelBatchBinaryExporter sınıfı, belirli bir seviye aralığındaki seviyeleri binary formatında dışa aktarmak için kullanılır.
// Sadece editörde kullanılmak üzere tasarlanmıştır.
public static class LevelBatchBinaryExporter
{
    public static void ExportLevelsToBinary(string levelsFolder, int startLevel, int endLevel, string outputFilePath)
    {
        var levelDatas = LoadLevelDatas(levelsFolder, startLevel, endLevel);
        WriteLevelsToBinary(levelDatas, outputFilePath);
        Debug.Log($"{levelDatas.Count} adet level {outputFilePath} dosyasına binary olarak export edildi.");
    }

    private static List<LevelData> LoadLevelDatas(string levelsFolder, int startLevel, int endLevel)
    {
        var levelDatas = new List<LevelData>();
        var files = Directory.GetFiles(levelsFolder, "level*_updated");
        foreach (var file in files)
        {
            if (!TryParseLevelFile(file, startLevel, endLevel, out LevelData data)) continue;
            levelDatas.Add(data);
        }
        return levelDatas;
    }

    private static bool TryParseLevelFile(string file, int startLevel, int endLevel, out LevelData data)
    {
        data = default;
        var fileName = Path.GetFileNameWithoutExtension(file);
        var parts = fileName.Split('_');
        if (parts.Length < 2) return false;
        if (!int.TryParse(parts[1], out int levelNum)) return false;
        if (levelNum < startLevel || levelNum > endLevel) return false;
        string json = File.ReadAllText(file);
        data = JsonConvert.DeserializeObject<LevelData>(json);
        return true;
    }

    private static void WriteLevelsToBinary(List<LevelData> levelDatas, string outputFilePath)
    {
        using (var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write(levelDatas.Count); //değişken olabilir dosyaya yazmak daha sağlıklı
            foreach (var level in levelDatas)
            {
                LevelDataBinaryWriter.WriteLevelData(bw, level);
            }
        }
    }
}
