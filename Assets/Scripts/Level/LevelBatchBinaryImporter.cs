using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LevelBatchBinaryImporter
{
    public static List<LevelData> ImportLevelsFromBinary(string binaryFilePath)
    {
        var levels = new List<LevelData>();
        using (var fs = new FileStream(binaryFilePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                LevelData data = new LevelData();
                data.Level = br.ReadInt32();
                data.LevelId = br.ReadString();
                data.Difficulty = br.ReadString();
                data.GridSize = br.ReadInt32();
                int boardRows = br.ReadInt32();
                data.Board = new string[boardRows][];
                for (int r = 0; r < boardRows; r++)
                {
                    int rowLen = br.ReadInt32();
                    data.Board[r] = new string[rowLen];
                    for (int c = 0; c < rowLen; c++)
                        data.Board[r][c] = br.ReadString();
                }
                levels.Add(data);
            }
        }
        Debug.Log($"{levels.Count} adet level import edildi.");
        return levels;
    }
}
