using System.Collections.Generic;
using System.IO;

public static class LevelBatchBinaryImporter
{
    public static List<LevelData> ImportLevelsFromLevelBatch(string binaryFilePath)
    {
        // for editor parse test
        using (var fs = new FileStream(binaryFilePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            return ReadLevelBatchFile(fs, br);
        }
    }

    public static List<LevelData> ImportLevelsFromStream(Stream stream, BinaryReader br)
    {
        return ReadLevelBatchFile(stream, br);
    }

    public static LevelData Deserialize(string filePath)
    {
        // single level read operation, FileStream is more performant
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var reader = new BinaryReader(fs))
        {
            return ReadLevelData(reader);
        }
    }

    private static List<LevelData> ReadLevelBatchFile(Stream stream, BinaryReader br)
    {
        // Do not cache if the downloaded file is corrupted
        var levels = new List<LevelData>();
        long fileLength = stream.Length;
        int count = br.ReadInt32();

        if (!IsValidLevelCount(count))
        {
            return new List<LevelData>();
        }

        for (int i = 0; i < count; i++)
        {
            long before = stream.Position;
            LevelData data = ReadLevelData(br);
            long after = stream.Position;
            if (after > fileLength || after <= before)
            {
                return new List<LevelData>();
            }
            levels.Add(data);
        }

        if (stream.Position != fileLength)
        {
            return new List<LevelData>();
        }

        return levels;
    }

    private static LevelData ReadLevelData(BinaryReader reader)
    {
        LevelData data = new LevelData
        {
            Level = reader.ReadInt32(),
            LevelId = reader.ReadString(),
            Difficulty = reader.ReadString(),
            GridSize = reader.ReadInt32()
        };
        int rows = reader.ReadInt32();
        data.Board = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            int cols = reader.ReadInt32();
            var row = new string[cols]; // single-dimensional array has less memory allocation
            for (int j = 0; j < cols; j++)
            {
                row[j] = reader.ReadString();
            }
            data.Board[i] = row;
        }
        return data;
    }
    private static bool IsValidLevelCount(int count)
    {
        return count > 0 && count <= Constants.MaxLevelBatchCount;
    }
}
