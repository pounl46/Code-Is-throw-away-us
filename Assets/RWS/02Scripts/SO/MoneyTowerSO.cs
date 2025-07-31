using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MoneyTowerSO", menuName = "Scriptable Objects/MoneyTowerSO")]
public class MoneyTowerSO : ScriptableObject
{
    [field : SerializeField] public Direction8 Direction { get; private set; }
    [field : SerializeField] public string TowerName { get; private set; }
    [field : SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField, Range(1, 3)] public int Distance { get; private set; } = 1;
    [field: SerializeField] public int Money { get; private set; } = 1;     //¹ö´Â µ·
}

[Flags, Serializable]
public enum Direction8
{
    None = 0,
    Up = 1 << 0,        // 0000 0001
    UpRight = 1 << 1,   // 0000 0010
    Right = 1 << 2,     // 0000 0100
    DownRight = 1 << 3, // 0000 1000
    Down = 1 << 4,      // 0001 0000
    DownLeft = 1 << 5,  // 0010 0000
    Left = 1 << 6,      // 0100 0000
    UpLeft = 1 << 7,    // 1000 0000
}