using System.IO;

public static class LevelFileHelper
{
    public static void WriteLevelData(BinaryWriter bw, LevelData levelData)
    {
        bw.Write(levelData.Level);
        bw.Write(levelData.LevelId ?? "");
        bw.Write(levelData.Difficulty ?? "");
        bw.Write(levelData.GridSize);
        bw.Write(levelData.Board.Length);
        foreach (var row in levelData.Board)
        {
            bw.Write(row.Length);
            foreach (var cell in row)
                bw.Write(cell ?? "");
        }
    }

    public static string GetLevelFilePath(int level, string persistentFolderPath)
    {
        string fileName = $"{Constants.LevelDataPath}{level}{Constants.LevelDataFileExtension}";
        return Path.Combine(persistentFolderPath, fileName);
    }
}

