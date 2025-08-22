using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelBatchBinaryImporter
{
    #region Import
    public static List<LevelData> ImportLevelsFromBinary(string binaryFilePath)
    {
        var levels = new List<LevelData>();

        using (var fs = new FileStream(binaryFilePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            long fileLength = fs.Length;
            int count = br.ReadInt32();

            if (!IsValidLevelCount(count))
            {
                Debug.LogError($"Level batch dosyası bozuk: count={count}, dosya={binaryFilePath}");
                return new List<LevelData>();
            }

            for (int i = 0; i < count; i++)
            {
                long before = fs.Position;
                LevelData data = ReadLevelData(br);
                long after = fs.Position;
                if (after > fileLength || after <= before)
                {
                    Debug.LogError($"Level batch dosyası bozuk: veri sonu hatası, dosya={binaryFilePath}");
                    return new List<LevelData>();
                }
                levels.Add(data);
            }

            if (fs.Position != fileLength)
            {
                Debug.LogWarning($"Level batch dosyası beklenenden büyük: {fileLength - fs.Position} fazla byte, dosya={binaryFilePath}");
                return new List<LevelData>();
            }
        }

        Debug.Log($"{levels.Count} adet level import edildi. Dosya: {binaryFilePath}");
        return levels;
    }

    private static bool IsValidLevelCount(int count)
    {
        return count > 0 && count <= Constants.MaxLevelBatchCount;
    }
    #endregion

    public static LevelData Deserialize(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        using (var ms = new MemoryStream(bytes))
        using (var reader = new BinaryReader(ms))
        {
            return ReadLevelData(reader);
        }
    }

    private static LevelData ReadLevelData(BinaryReader reader)
    {
        LevelData data = new LevelData();
        data.Level = reader.ReadInt32();
        data.LevelId = reader.ReadString();
        data.Difficulty = reader.ReadString();
        data.GridSize = reader.ReadInt32();
        int rows = reader.ReadInt32();
        data.Board = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            int cols = reader.ReadInt32();
            data.Board[i] = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                data.Board[i][j] = reader.ReadString();
            }
        }
        return data;
    }
    
}
