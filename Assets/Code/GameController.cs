using Assets.Code.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {

    private static GameController _instance;
    
    public bool PlayerInDungeon { get; set; }  //stop chunkprocessing if in dungeonspace
    public int TriggeredDungeonSeed { get; set; }  //seed of dungeon that was triggered to generate

    #region Dungeon Related Components
    public Vector3 TiggeredDungeonEnterance { get; set; }  // position of door we entered through

    //store the seed of completed dungeons so we can prevent key spawning
    public List<int> CompletedDungeonList { get; set; } 
    public int BaseSeed { get; set; }
    public DungeonGenerator DungeonGenerator {get; set;}
    #endregion

    #region Player Related Components
    public PlayerController PlayerController { get; set; }
    #endregion


    //Singleton pattern
    public static GameController GetSharedInstance()
    {
        if(_instance == null)
        {
            _instance = new GameController();
            _instance.BaseSeed = 20180321;
            _instance.DungeonGenerator = new DungeonGenerator();
            _instance.CompletedDungeonList = new List<int>();
        }
        return _instance;
    }
}
