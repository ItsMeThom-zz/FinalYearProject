using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Generator;
using LibNoise.Operator;


public class PoissonDiscSampler// : MonoBehaviour
{

    private float min_radius;
    private float offset;
    private ArrayList grid;
    private ArrayList active;
    private int rows, cols;
    private double cell_width;


    public  int attempts;
    private int chunkSize = 129;

    public int Radius { get; set; }

    float[,] placementMap;

    public PoissonDiscSampler()
    {

        this.Radius = 8;
       
        attempts = 20;

        grid = new ArrayList();
        active = new ArrayList();
        cell_width = Radius / Math.Sqrt(2);

        cols = (int)(chunkSize / cell_width);
        rows = (int)(chunkSize / cell_width);

        for (var n = 0; n < cols * rows; n++)
        {
            grid.Add(null);

        }
    }



    public ArrayList GeneratePoints(int size)
    {
        this.chunkSize = size; //get from TerChunk settings

        //centerpoints
        int rndX = chunkSize / 2;
        int rndY = chunkSize / 2;
       
        int i = (int)(rndX / cell_width);
        int j = (int)(rndY / cell_width);
        Vector3 pos = new Vector3(rndX, 0, rndY);

        grid[i + j * cols] = pos;
        active.Add(pos);
        createPoints();

        return grid;
    }


    private void createPoints()
    {
        while (active.Count > 0)
        {
            Vector3 sample;
            int randIndex = UnityEngine.Random.Range(0, active.Count);
            Vector3 position = (Vector3)active[randIndex];
            Boolean found = false;


            for (var p = 0; p < attempts; p++)
            {
                float angle = UnityEngine.Random.Range(0, 2 * (float)Math.PI);
                offset = UnityEngine.Random.Range(Radius, 2 * Radius);
                float offsetX = (float)Math.Cos(angle) * offset;
                float offsetZ = (float)Math.Sin(angle) * offset;

                sample = new Vector3(offsetX + position.x, 0, offsetZ + position.z);

                //check this x,z in the passed heightmap. if < sealevel, just fuck right off.
                
                int col = (int)(sample.x / cell_width);
                int row = (int)(sample.z / cell_width);
               
             
                if (col > 0 && row > 0 && col < cols && row < rows)
                {
                    Boolean ok = true;

                    for (int g = -1; g <= 1; g++)
                    {
                        for (int h = -1; h <= 1; h++)
                        {
                            int index = (col + g) + (row + h) * cols;
                            if (index > 0 && index < rows * cols)
                            {
                                var neighbour = grid[index];
                                if (neighbour != null)
                                {
                                    float dist = Vector3.Distance(sample, (Vector3)neighbour);
                                    if (dist < Radius)
                                    {
                                        ok = false;
                                    }
                                }
                            }

                        }
                    }
                    if (ok)
                    {
                        found = true;
                        grid[col + row * cols] = sample;
                        active.Add(sample);
                    }

                }

            }

            if (!found)
            {
                active.RemoveAt(randIndex);
            }
        }
    }

}


