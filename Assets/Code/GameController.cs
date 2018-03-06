﻿using System.Collections;
using TerrainGenerator;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private const int Radius = 4;

    public Vector2i PreviousPlayerChunkPosition;

    public Transform Player;
    public Transform WaterPlane;
    public TerrainChunkGenerator Generator;

    public Button StartButton;

    public Renderer MapRenderer;

    public bool DEV = false;
    public bool IsReady = false;

    public void StartAll()
    {
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
        this.IsReady = true;
        RenderWorldMap();
    }

    private void Update()
    {
        if (Player.gameObject.activeSelf)
        {
            var playerChunkPosition = Generator.GetChunkPosition(Player.position);
            if (!playerChunkPosition.Equals(PreviousPlayerChunkPosition))
            {
                Generator.UpdateTerrain(Player.position, Radius);
                PreviousPlayerChunkPosition = playerChunkPosition;
            }
        }
    }

    private void RenderWorldMap()
    {
        
        //var worldgen = GameObject.FindObjectOfType<TerrainChunkGenerator>().WorldGenerator;
        //Texture2D tex = CreateTexture(worldgen.ElevationData);
        //var mapcam = GameObject.Find("WorldMap");
        //var textureRender = mapcam.GetComponent<Renderer>().material.mainTexture = tex;

        //MapRenderer.sharedMaterial.mainTexture = textureRender;
        //MapRenderer.transform.localScale = new Vector3(textureRender.width, 1, textureRender.height);
    }

    private Texture2D CreateTexture(float[,] noisemap)
    {
        int width = noisemap.GetLength(0);
        int height = noisemap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.LerpUnclamped(Color.black, Color.white, noisemap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();

        return texture;

        
        
    }
}