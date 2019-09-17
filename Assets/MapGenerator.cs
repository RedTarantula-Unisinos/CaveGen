using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    public int width =120,height=60,its = 10;

    public string seed;
    public bool useRandomSeed = true;

    [Range(0,100)]
    public int randomFIllPercent = 40;
    int[,] map;
    public int nWalls = 4;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }
    
    public void GenerateMap()
    {
        map = new int[width,height];
        RandomFillMap();

        for (int i = 0 ; i < its ; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }
        System.Random prng = new System.Random(seed.GetHashCode());

        for (int x = 0 ; x < width ; x++)
        {
            for (int y = 0 ; y < height ; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x,y] = 1;
                }
                else
                {


                    map[x,y] = (prng.Next(0,100) < randomFIllPercent) ? 1 : 0;
                }

            }
        }
    }


    void SmoothMap()
    {
        for (int x = 0 ; x < width ; x++)
        {
            for (int y = 0 ; y < height ; y++)
            {
                int neighbourWalls = GetNeighbourWallCount(x,y);
                if(neighbourWalls > nWalls)
                {
                    map[x,y] = 1;
                }
                
                    else if (neighbourWalls < nWalls)
                {
                    map[x,y] = 0;
                }
            }
        }


    }

    int GetNeighbourWallCount(int gridx,int gridy)
    {
        int wallCount = 0;
        for (int neighbourx = gridx - 1 ; neighbourx <= gridx + 1 ; neighbourx++)
        {
            for (int neighboury = gridy - 1 ; neighboury <= gridy + 1 ; neighboury++)

            {
                if (neighbourx >= 0 && neighbourx < width && neighboury >= 0 && neighboury < height)
                {
                    if (neighbourx != gridx || neighboury != gridy)
                    {
                        wallCount += map[neighbourx,neighboury];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void OnDrawGizmos()
    {
        if(map != null)
        {
            for (int x = 0 ; x < width ; x++)
            {
                for (int y = 0 ; y < height ; y++)
                {
                    Gizmos.color = (map[x,y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f,0,-height / 2 + y + .5f);
                    Gizmos.DrawCube(pos,Vector3.one);
                }
            }
        }
    }
}
