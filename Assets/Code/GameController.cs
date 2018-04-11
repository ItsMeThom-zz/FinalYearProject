using Assets.Code.Player;
using Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController {

    private static GameController _instance;




    #region Dungeon Related Components
    // position of door we entered through
    public Vector3 TiggeredDungeonEnterance { get; set; } 
    //seed of dungeon that was triggered to generate
    public int TriggeredDungeonSeed { get; set; } 
    //store the seed of completed dungeons so to prevent respawning keys
    public List<int> CompletedDungeonList { get; set; } 
    public int BaseSeed { get; set; }
    public DungeonGenerator DungeonGenerator {get; set;}
    #endregion

    #region Player Related Components
    public PlayerController PlayerController { get; set; }
    public bool PlayerInCombat { get; set; }
    public bool PlayerInDungeon { get; set; }  //stop chunkprocessing if in dungeonspace
    #endregion

    #region Music Related Components and Properties
    public MusicGenerator MusicGenerator { get; set; }
    

    #endregion
    //Singleton pattern
    public static GameController GetSharedInstance()
    {
        if(_instance == null)
        {
            _instance = new GameController();
            _instance.BaseSeed = 20180321; //default seed if none set
            _instance.DungeonGenerator = new DungeonGenerator();
            _instance.CompletedDungeonList = new List<int>();
            _instance.PlayerInCombat = false;
            _instance.PlayerInDungeon = false;
        }
        return _instance;
    }
}
