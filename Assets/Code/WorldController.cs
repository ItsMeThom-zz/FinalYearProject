using System.Collections;
using System.Linq;
using TerrainGenerator;
using UnityEngine;
using UnityEngine.UI;
using Utils;

/// <summary>
/// Initalises world generation, and terrain chunk generation components
/// Holds data related to the world
/// </summary>
public class WorldController : MonoBehaviour
{
    public GameController GameData;
    private const int Radius = 4;

    public Vector2i PreviousPlayerChunkPosition;

    public Transform Player;
    public Transform WaterPlane;
    public TerrainChunkGenerator Generator;

    public Button StartButton;

    //player camera rendering while the world is being generated
    public GameObject LoadingCamera;

    public bool DEV = false;
    public bool IsReady = false;

    int threadCacheCount = 0;
    private void Awake()
    {
        this.GameData = GameController.GetSharedInstance();
    }
    public void StartAll()
    {
        GameData.PlayerInDungeon = false; //start in the world

        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        var canActivateCharacter = false;

        StartButton.interactable = false;
        Generator.UpdateTerrain(Player.position, Radius);

        do
        {
            var exists = Generator.IsTerrainAvailable(Player.position);
            if (exists)
                canActivateCharacter = true;
            yield return null;
        } while (!canActivateCharacter);

        PreviousPlayerChunkPosition = Generator.GetChunkPosition(Player.position);
        Player.position = new Vector3(Player.position.x, Generator.GetTerrainHeight(Player.position) + 0.5f, Player.position.z);
        Player.gameObject.SetActive(true);
        WaterPlane.gameObject.SetActive(true);
        LoadingCamera.GetComponentInChildren<Camera>().enabled = false;
        LoadingCamera.GetComponentInChildren<Canvas>().enabled = false;
        this.IsReady = true;
        
    }

    private void Update()
    {
        int currentCount = ((IEnumerable)System.Diagnostics.Process.GetCurrentProcess().Threads)
            .OfType<System.Diagnostics.ProcessThread>()
            .Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running)
            .Count();

        if (threadCacheCount != currentCount)
        {
            threadCacheCount = currentCount;
            Debug.Log("Threads Active: " + threadCacheCount);
        }
        
        if (Player.gameObject.activeSelf)
        {
            //only update chunks if player is not InDungeon
            if (!GameData.PlayerInDungeon)
            {
                var playerChunkPosition = Generator.GetChunkPosition(Player.position);
                if (!playerChunkPosition.Equals(PreviousPlayerChunkPosition))
                {
                    Generator.UpdateTerrain(Player.position, Radius);
                    PreviousPlayerChunkPosition = playerChunkPosition;
                }
            }

        }
    }
}