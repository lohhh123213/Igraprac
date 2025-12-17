using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string playerName = "Player";
    public int coins = 0;
    public List<string> ownedUpgrades = new List<string>();
}

