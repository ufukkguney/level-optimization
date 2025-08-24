public static class Constants
{
    #region Difficulty Strings
    public const string DifficultyEasy = "easy";
    public const string DifficultyMedium = "medium";
    public const string DifficultyHard = "hard";
    #endregion

    #region Button Colors
    public static readonly UnityEngine.Color EasyColor = UnityEngine.Color.green;
    public static readonly UnityEngine.Color MediumColor = UnityEngine.Color.yellow;
    public static readonly UnityEngine.Color HardColor = UnityEngine.Color.red;
    public static readonly UnityEngine.Color DefaultButtonColor = UnityEngine.Color.white;
    #endregion

    #region Level Batch
    public const int MaxLevelBatchCount = 1000;
    public const int LevelCheckOffset = 10;
    public const int LevelBatchSize = 25;
    #endregion

    #region Level File Paths
    public const string SaveFolder = "DownloadedLevels";
    public const string LevelsFolder = "Assets/ProjectRoot/Levels";
    public const string OutputFile = "Assets/ProjectRoot/LevelsBatch/levels_1_25.bin";
    public const string BinaryFile = "Assets/ProjectRoot/LevelsBatch/levels_1_25.bin";
    public const string SingleLevelFile = "test";
    public const string ResourcesLevelFile = "level_";
    #endregion

    #region Level Data Format
    public const string LevelDataFileExtension = ".bin";
    public const string LevelDataPath = "LevelData_";
    #endregion

    #region User Defaults
    public const string LevelIdPrefKey = "User_LevelId";
    public const int DefaultLevelId = 0;
    #endregion

    #region Level Range
    public const int StartLevel = 1;
    public const int EndLevel = 25;
    #endregion

    #region File I/O
    public const int BufferSize = 4096;
    #endregion
}

