using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    public int width =20,height=20,its = 10;

    public GameObject wall, floor;

    public GameObject physicalMap;

    public string seed;
    public bool useRandomSeed = true;

    [Range(0,100)]
    public int randomFIllPercent = 40;
    [SerializeField]
    public int[,] map;
    public int nWalls = 4;
    public Vector2 mainAreaCoord;
    [SerializeField]
    public List<Vector2> coordsToTest;
    public List<Vector2> mainAreaPositions;
    int ffCount = 0;
    bool ffDone = false;
    [Range(0f,100f)]
    public float emptyChance = 60f;
    bool built=false;

    [Serializable]
    public struct DungeonCell
    {
        public bool wall;
        public GameObject agent;
        public GameObject go;

        public DungeonCell(bool isWall,GameObject self, GameObject obj = null)
        {
            agent = obj;
            wall = isWall;
            go = self;
        }
    }

    [Serializable]
    public struct DungeonAgent
    {
        public string debugName;
        public GameObject obj;
        [Range(0f,100f)]
        public float chance;

        public DungeonAgent(float chanceNum,GameObject gobj,string name)
        {
            obj = gobj;
            chance = chanceNum;
            debugName = name;
        }
    }
    [SerializeField]
    public DungeonAgent[] instancesToFillDungeon;
    [SerializeField]
    public DungeonAgent playerAgent;
    public bool spawnedPlayer=false;
    [SerializeField]
    public DungeonCell[,] cells;
    List<string> debugAgents;

    void Start()
    {
        GenerateMap();
    }

    void ResetStuff()
    {
        cells = new DungeonCell[width,height];
        coordsToTest = new List<Vector2>();
        mainAreaPositions = new List<Vector2>();
        ffCount = 0;
        ffDone = false;
        built = false;
        spawnedPlayer = false;

    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    GenerateMap();
        //}
    }

    public void ClearPhysicalMap()
    {
        DestroyImmediate(physicalMap);
        physicalMap = new GameObject();
    }

    void GetCellsLayout()
    {
        for (int x = 0 ; x < width ; x++)
        {
            for (int y = 0 ; y < height ; y++)
            {
                //Debug.Log("Map " + x.ToString() + "x" + y.ToString() + ": " + map[x,y].ToString());
                if (map[x,y] == 1)
                {
                    cells[x,y].wall = true;
                }
                else
                {
                    cells[x,y].wall = false;
                }

            }
        }
    }

    public void BuildPhysicalMap()
    {
        GetCellsLayout();
        for (int x = 0 ; x < width ; x++)
        {
            for (int y = 0 ; y < height ; y++)
            {
                GameObject go = SpawnWallFloor(new Vector2(x,y),cells[x,y].wall);
                if(cells[x,y].agent != null)
                {
                    GameObject child = Instantiate(cells[x,y].agent,go.transform);
                    child.transform.localPosition = new Vector3(0f,0f,-0.5f);
                }

                
            }
        }
        built = true;
    }
    
    public void GenerateMap()
    {
        Debug.LogWarning("GENERATING");
        ResetStuff();
         mainAreaCoord = new Vector2(-1,-1);
        if (physicalMap == null)
            physicalMap = new GameObject();
        
        cells = new DungeonCell[width,height];

        map = new int[width,height];
        RandomFillMap();

        for (int i = 0 ; i < its ; i++)
        {
            SmoothMap();
            string mapString = "Iteration: " + i.ToString() + "\n";
            for (int x = 0 ; x < width ; x++)
            {
                for (int y = 0 ; y < height ; y++)
                {
                    mapString += map[x,y].ToString() + ", ";
                }
                mapString += "\n";
            }

            GetMainArea();
            Debug.LogWarning("FINISHED GENERATING");
            //Debug.Log(mapString);
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

    void GetMainArea()
    {
        int largestAreaSize = 0;
        int largestAreaX = -1;
        int largestAreaY = -1;
        
       coordsToTest = new List<Vector2>();

        for (int x = 0 ; x < width ; x++)
        {
            for (int y = 0 ; y < height ; y++)
            {
                if(map[x,y] == 0)
                {
                    coordsToTest.Add(new Vector2(x,y));
                
                }
            }
        }
        List<Vector3> otherAreas = new List<Vector3>();
        while (coordsToTest.ToArray().Length > 0)
        {
            Vector3 testedArea = GetAreaSize(coordsToTest[0]);
            if (testedArea.z > largestAreaSize)
            {
                largestAreaSize = (int)testedArea.z;
                largestAreaX = (int)testedArea.x;
                largestAreaY = (int)testedArea.y;
            }
            else
            {
                otherAreas.Add(testedArea);
                FillOtherAreas(testedArea);
            }
        }
        

        mainAreaCoord = new Vector2(largestAreaX,largestAreaY);
        MainArea();

        //Debug.Log(coordsToTest);
    }

    Vector3 GetAreaSize(Vector2 coord)
    {
        ffCount = 0;
        GetMainArea(coord);
        Vector3 v = new Vector3(coord.x,coord.y,ffCount);
        //Debug.Log("Returning area");
        return v;
    }

    void GetMainArea(Vector2 coord) // Flood fill
    {
        if (map[(int)coord.x,(int)coord.y] == 0)
        {
            coordsToTest.Remove(coord);
            mainAreaPositions.Add(coord);
            ffCount++;
            int cx = (int)coord.x;
            int cy = (int)coord.y;
            Vector2[] dirs = { new Vector2(cx + 1,cy),new Vector2(cx,cy + 1),new Vector2(cx - 1,cy),new Vector2(cx,cy - 1),  new Vector2(cx-1,cy-1), new Vector2(cx-1,cy+1), new Vector2(cx+1,cy-1), new Vector2(cx+1,cy+1)};

            foreach (Vector2 dir in dirs)
            {
                if (coordsToTest.Contains(dir))
                { GetMainArea(dir); }
            }
        }
    }

    void MainArea()
    {
        //Debug.Log("Running Main Area");
        float allAgentsChance = 0f;
        debugAgents = new List<string>();
        foreach (DungeonAgent da in instancesToFillDungeon)
        {
            allAgentsChance += da.chance;
        }

        float totalChances = emptyChance + allAgentsChance;
        //Debug.Log(totalChances.ToString());
        //FillMainArea(coord);
        int ranTimes = 0;

        // Spawn Player
        if (!spawnedPlayer)
        {
            int rpos = UnityEngine.Random.Range(0,mainAreaPositions.ToArray().Length);
            Vector2 randomPos = mainAreaPositions[rpos];
            mainAreaPositions.Remove(randomPos);
            GameObject playerGO = cells[(int)randomPos.x,(int)randomPos.y].agent = playerAgent.obj;
            playerGO.GetComponent<PlayerScript>().cellPosition = new Vector2((int)randomPos.x,(int)randomPos.y);

            spawnedPlayer = true;
        }
        //

        foreach (Vector2 pos in mainAreaPositions)
        {
            ranTimes++;
            float f = UnityEngine.Random.Range(0f,totalChances);
            if(f > emptyChance)
            {
                float chanceFloor = emptyChance;
                foreach (DungeonAgent da in instancesToFillDungeon)
                {
                    chanceFloor += da.chance;
                    if(f < chanceFloor)
                    {
                        debugAgents.Add("F(" + f.ToString() + ") Added agent " + da.debugName + " to coordinate: " + pos);
                        //Debug.Log(da.obj.transform.name);
                        //Debug.Log(pos.x.ToString() + "x" + pos.y.ToString());
                        //Debug.Log(cells[(int)pos.x,(int)pos.y].agent == null);
                        //Debug.Log(cells[(int)pos.x,(int)pos.y].wall);
                        cells[(int)pos.x,(int)pos.y].agent = da.obj;
                        break;
                    }
                }
            }
            else
            {
                debugAgents.Add("F(" + f.ToString() + ") Empty space");
            }
        }
        //Debug.Log("Ran " + ranTimes.ToString() + " with " + mainAreaPositions.ToArray().Length.ToString() + " total positions");
        
    }

    void SpawnAgentOnPosition(Transform parent,GameObject obj)
    {
        GameObject go = Instantiate(obj,parent);
        Debug.Log(go);
    }
    GameObject SpawnWallFloor(Vector2 pos, bool wallBool = false)
    {
        Debug.Log(wallBool);
        GameObject go;
        if (wallBool)
            go = wall;
        else
            go = floor;
        GameObject mapElement = Instantiate(go,physicalMap.transform);
        Vector3 objPos = mapElement.transform.position;
        objPos = new Vector3(pos.x,0,pos.y);
        mapElement.transform.position = objPos;
        cells[(int)pos.x,(int)pos.y].go = mapElement;

        return mapElement;
    }

    void FillMainArea(Vector2 coord)
    {
        List<Vector2> dirs = new List<Vector2>();
        dirs.Add(new Vector2(coord.x + 1,coord.y));
        dirs.Add(new Vector2(coord.x,coord.y + 1));
        dirs.Add(new Vector2(coord.x - 1,coord.y));
        dirs.Add(new Vector2(coord.x,coord.y - 1));

        foreach (Vector2 dir in dirs)
        {
            if(map[(int)dir.x,(int)dir.y] == 0 && !mainAreaPositions.Contains(dir))
            {
                if (dir.x >= 0 && dir.x < width && dir.y >= 0 && dir.y < height)
                {
                    FillMainArea(dir);
                    mainAreaPositions.Add(dir);
                }
            }
        }
    }

    void FillOtherAreas(Vector2 coord)
    {
        Vector2[] dirs = { new Vector2(coord.x + 1,coord.y),new Vector2(coord.x,coord.y + 1),new Vector2(coord.x - 1,coord.y),new Vector2(coord.x,coord.y - 1) };
        foreach (Vector2 dir in dirs)
        {
            map[(int)dir.x,(int)dir.y] = 1;
            
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
        if(map != null && !built)
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
