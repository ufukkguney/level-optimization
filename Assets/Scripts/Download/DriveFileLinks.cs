using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DriveFileLinks", menuName = "LevelOptimization/DriveFileLinks")]
public class DriveFileLinks : ScriptableObject
{
    [Tooltip("Google Drive dosya ID veya linkleri")]
    public List<string> FileLinks = new List<string>();
}
