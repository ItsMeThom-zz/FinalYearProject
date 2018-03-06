using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData{

    public static GameData _instance;

    public int TriggeredDungeonSeed;            //seed of dungeon that was triggered to generate
    public Transform TiggeredDungeonEnterance;  // position of door we entered through
    //Singleton pattern
    public static GameData Get()
    {
        if(_instance == null)
        {
            _instance = new GameData();
        }
        return _instance;
    }
}
