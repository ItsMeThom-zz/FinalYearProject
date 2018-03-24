using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {

    public static GameController _instance;

    public bool PlayerInDungeon { get; set; }  //stop chunkprocessing if in dungeonspace
    public int TriggeredDungeonSeed { get; set; }  //seed of dungeon that was triggered to generate
    public Vector3 TiggeredDungeonEnterance { get; set; }  // position of door we entered through


    public int BaseSeed { get; set; }
    public DungeonGenerator DungeonGenerator {get; set;}
    //Singleton pattern
    public static GameController GetSharedInstance()
    {
        if(_instance == null)
        {
            _instance = new GameController();
            _instance.BaseSeed = 20180321;
            _instance.DungeonGenerator = new DungeonGenerator();
        }
        return _instance;
    }
}
