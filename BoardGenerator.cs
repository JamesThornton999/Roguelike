using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardGenerator : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int xSectors = 5;
    public int ySectors = 5;
    public int buffer = 2;
    public int genSize = 10;
    public int xRoomMin = 3;
    public int yRoomMin = 3;
    public int xRoomLengthMax = 8;
    public int yRoomLengthMax = 8;

    public LayerMask floorLayer;
    public LayerMask wallLayer;
    public GameObject[] dirtFloorTiles;
    public GameObject[] tileFloorTiles;
    public GameObject[] wallTiles;
    public GameObject[] doors;
    public GameObject upStairs;
    public GameObject downStairs;

    private Transform boardHolder;
    private List<Transform> allRoomFloors = new List<Transform>(); //use transforms instead of GameObjects.
    private List<Transform>[,] roomFloors; //Same as Above.
    private List<Vector3> roomBorders = new List<Vector3>();
    public List<Transform> hallTiles = new List<Transform>();
    private List<Transform> wallUnits = new List<Transform>();
    private List<int> rooms = new List<int>();
    private List<int> connectedRooms = new List<int>();

    private int currXCoordinate = 0;
    private int currYCoordinate = 0;

    public void prepBorders()
    {
        roomFloors = new List<Transform>[xSectors, ySectors];
        for (int x = 0; x < xSectors; x++)
            for (int y = 0; y < ySectors; y++)
            {
                roomFloors[x, y] = new List<Transform>();
                roomFloors[x, y].Clear();
            }

    }
    void Awake()
    {
        //prepBorders();
        //BoardSetup(0);
    }
    public void BoardSetup(int wallStyle)
    {
        allRoomFloors.Clear();
        GameObject hallTile = tileFloorTiles[Random.Range(0, tileFloorTiles.Length)];
        boardHolder = new GameObject("Board").transform;
        //This places a room in each of the sectors. 
        for (int y = 0; y < ySectors; y++)
        {
            for (int x = 0; x < xSectors; x++)
            {
                //we start at 2,2, genning until 12,12, sector ends at 14,14
                currXCoordinate = (x * (genSize + (buffer * 2))) + 2;
                currYCoordinate = (y * (genSize + (buffer * 2))) + 2;
                int xSize = Random.Range(xRoomMin, xRoomLengthMax);
                int ySize = Random.Range(yRoomMin, yRoomLengthMax);
                int xStart = Random.Range(currXCoordinate, (currXCoordinate + genSize - (xSize - 1)));
                int yStart = Random.Range(currYCoordinate, (currYCoordinate + genSize - (ySize - 1)));
                int floorStyle = Random.Range(0, dirtFloorTiles.Length);
                rooms.Add((y * 5) + x);
                for (int yGen = yStart - 1; yGen <= yStart + ySize + 1; yGen++)
                {
                    for (int xGen = xStart - 1; xGen <= xStart + xSize + 1; xGen++)
                    {
                        int Rand2;
                        GameObject toInstantiate = dirtFloorTiles[floorStyle];
                        bool go = true;
                        bool wall = false;
                        if (xGen == xStart - 1 || xGen == xStart + xSize + 1 || yGen == yStart - 1 || yGen == yStart + ySize + 1)
                            if ((xGen == xStart - 1 || xGen == xStart + xSize + 1) && (yGen == yStart - 1 || yGen == yStart + ySize + 1))
                            {
                                wall = true;
                                //This should be a corner, so just place a wall.
                                Rand2 = Random.Range(0, 4);
                                if (Rand2 == 0)
                                {
                                    toInstantiate = wallTiles[0];
                                }
                                else
                                    toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                            }
                            else
                            {
                                go = false;
                                roomBorders.Add(new Vector3(xGen, yGen, 0f));
                            }

                        if (go)
                        {
                            GameObject instance = Instantiate(toInstantiate, new Vector3(xGen, yGen, 0f), Quaternion.identity) as GameObject;
                            instance.transform.SetParent(boardHolder);
                            if (!wall)
                            {
                                allRoomFloors.Add(instance.transform);
                                roomFloors[x, y].Add(instance.transform);
                            }else
                            {
                                wallUnits.Add(instance.transform);
                            }

                        }

                    }
                }
            }
        }
        //Make sure every room has at least one connection.
        //Seed the procedure by choosing a room at random.
        int Rand = Random.Range(0, rooms.Count);
        connectedRooms.Add(rooms[Rand]);
        rooms.RemoveAt(Rand);
        while (rooms.Count != 0)
        {
            int startRoom = connectedRooms[Random.Range(0, connectedRooms.Count)];
            int endRand = Random.Range(0, rooms.Count);
            int endRoom = rooms[endRand];
            runHalls(startRoom, endRoom, hallTile);
            connectedRooms.Add(rooms[endRand]);
            rooms.RemoveAt(endRand);
        }
        addWalls();
        for(int i = 0; i < wallUnits.Count; i++)
        {
            wallUnits[i].gameObject.SendMessage("changeTile", wallStyle);
        }
        for (int i = 0; i < wallUnits.Count; i++)
        {
            wallUnits[i].gameObject.SendMessage("UpdateLayer", 10);
            PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(wallUnits[i].position.x), Mathf.FloorToInt(wallUnits[i].position.y));
            path.tile = wallUnits[i];
        }
        //Added to update floors to LOSlayer;
        for (int i = 0; i < allRoomFloors.Count; i++)
        {
            allRoomFloors[i].SendMessage("UpdateLayer", 10);
            PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(allRoomFloors[i].position.x), Mathf.FloorToInt(allRoomFloors[i].position.y));
            path.tile = allRoomFloors[i];
        }
        for(int i = 0; i < hallTiles.Count; i++)
        {
            hallTiles[i].SendMessage("UpdateLayer", 10);
            PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(hallTiles[i].position.x), Mathf.FloorToInt(hallTiles[i].position.y));
            path.tile = hallTiles[i];
        }
        int picked = Random.Range(0, allRoomFloors.Count);
        GameObject stairInstance = Instantiate(upStairs, new Vector3(allRoomFloors[picked].position.x, allRoomFloors[picked].position.y, 0f), Quaternion.identity) as GameObject;
        allRoomFloors.RemoveAt(picked);
        picked = Random.Range(0, allRoomFloors.Count);
        stairInstance = Instantiate(downStairs, new Vector3(allRoomFloors[picked].position.x, allRoomFloors[picked].position.y, 0f), Quaternion.identity) as GameObject;
        allRoomFloors.RemoveAt(picked);

    }


    void runHalls(int startRoom, int endRoom, GameObject hallTile)
    {
        int sectorY = startRoom / 5;
        int sectorX = startRoom % 5;
        Transform tile = roomFloors[sectorX, sectorY][Random.Range(0, roomFloors[sectorX, sectorY].Count)];
        sectorY = endRoom / 5;
        sectorX = endRoom % 5;
        Transform targetTile = roomFloors[sectorX, sectorY][Random.Range(0, roomFloors[sectorX, sectorY].Count)];
        genHall(tile, targetTile, hallTile);
    }

    void genHall(Transform tile, Transform targetTile, GameObject hallTile)
    {
        bool running = true;

        Vector2 prevMove;
        if (Random.Range(0, 2) == 0)
        {
            if (targetTile.position.x > tile.position.x)
                prevMove = new Vector2(1, 0);
            else
                prevMove = new Vector2(-1, 0);
        }
        else
        {
            if (targetTile.position.y > tile.position.y)
                prevMove = new Vector2(0, 1);
            else
                prevMove = new Vector2(0, -1);
        }
        int tries = 0;
        while (running)
        {
            int xMove = 0;
            int yMove = 0;
            if (targetTile.position.x > tile.position.x)
                xMove = 1;
            else if (targetTile.position.x < tile.position.x)
                xMove = -1;

            if (targetTile.position.y > tile.position.y)
                yMove = 1;
            else if (targetTile.position.y < tile.position.y)
                yMove = -1;

            Vector2 plottedMove = prevMove;
            RaycastHit2D hit = Physics2D.Linecast(tile.position, new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), wallLayer);
            RaycastHit2D floorHit;

            if (hit.transform == null && (Math.Abs(targetTile.position.x - (tile.position.x + plottedMove.x)) < Math.Abs(targetTile.position.x - (tile.position.x)) ||
                Math.Abs(targetTile.position.y - (tile.position.y + plottedMove.y)) < Math.Abs(targetTile.position.y - (tile.position.y))))
            {
                floorHit = Physics2D.Linecast(new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), new Vector2(tile.position.x + plottedMove.x + 0.25f, tile.position.y + plottedMove.y + 0.25f), floorLayer);
                if (floorHit.transform == null)
                {
                    GameObject instance = Instantiate(hallTile, new Vector3(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y, 0f), Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    tile = instance.transform;
                    hallTiles.Add(instance.transform);
                    bool xDir;
                    if (plottedMove.x != 0)
                        xDir = true;
                    else
                        xDir = false;
                    doorCheck((int)tile.position.x, (int)tile.position.y, xDir);
                }
                else
                {
                    tile = floorHit.transform;
                }
            }
            else
            {
                if (plottedMove.x != 0)
                {
                    plottedMove.x = 0;
                    if(yMove == 0)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            yMove = 1;
                        }
                        else
                            yMove = -1;
                    }
                    plottedMove.y = yMove;
                }
                else
                {
                    plottedMove.y = 0;
                    if (xMove == 0)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            xMove = 1;
                        }
                        else
                            xMove = -1;
                    }
                    plottedMove.x = xMove;
                }
                hit = Physics2D.Linecast(tile.position, new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), wallLayer);

                if (hit.transform == null && (Math.Abs(targetTile.position.x - (tile.position.x + plottedMove.x)) < Math.Abs(targetTile.position.x - tile.position.x) ||
                    Math.Abs(targetTile.position.y - (tile.position.y + plottedMove.y)) < Math.Abs(targetTile.position.y - tile.position.y)))
                {
                    floorHit = Physics2D.Linecast(new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), new Vector2(tile.position.x + plottedMove.x + 0.25f, tile.position.y + plottedMove.y + 0.25f), floorLayer);

                    if (floorHit.transform == null)
                    {
                        GameObject instance = Instantiate(hallTile, new Vector3(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                        tile = instance.transform;
                        hallTiles.Add(instance.transform);
                        bool xDir;
                        if (plottedMove.x != 0)
                            xDir = true;
                        else
                            xDir = false;
                        doorCheck((int)tile.position.x, (int)tile.position.y, xDir);
                    }
                    else
                    {
                        tile = floorHit.transform;
                    }
                }else
                {

                    if (plottedMove.x == 0)
                    {
                        plottedMove.y = yMove * -1;
                    }
                    else
                    {
                        plottedMove.x = xMove * -1;
                    }
                    hit = Physics2D.Linecast(tile.position, new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), wallLayer);
                    if (hit.transform == null)
                    {
                        //now we go this way!
                        //tile = layTile(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y, out floorHit, hallTile);
                        floorHit = Physics2D.Linecast(new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), new Vector2(tile.position.x + plottedMove.x + 0.25f, tile.position.y + plottedMove.y + 0.25f), floorLayer);

                        if (floorHit.transform == null)
                        {
                            GameObject instance = Instantiate(hallTile, new Vector3(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                            tile = instance.transform;
                            hallTiles.Add(instance.transform);
                            bool xDir;
                            if (plottedMove.x != 0)
                                xDir = true;
                            else
                                xDir = false;
                            doorCheck((int)tile.position.x, (int)tile.position.y, xDir);
                        }
                        else
                        {
                            tile = floorHit.transform;
                        }
                    }else
                    {
                        plottedMove = prevMove;
                        hit = Physics2D.Linecast(tile.position, new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), wallLayer);

                        if (hit.transform == null)
                        {
                            floorHit = Physics2D.Linecast(new Vector2(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y), new Vector2(tile.position.x + plottedMove.x + 0.25f, tile.position.y + plottedMove.y + 0.25f), floorLayer);

                            if (floorHit.transform == null)
                            {
                                GameObject instance = Instantiate(hallTile, new Vector3(tile.position.x + plottedMove.x, tile.position.y + plottedMove.y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                                tile = instance.transform;
                                hallTiles.Add(instance.transform);
                                bool xDir;
                                if (plottedMove.x != 0)
                                    xDir = true;
                                else
                                    xDir = false;
                                doorCheck((int)tile.position.x, (int)tile.position.y, xDir);
                            }
                            else
                            {
                                tile = floorHit.transform;
                            }
                        }else
                        {
                            Debug.Log("Path Error!");
                            running = false;
                        }

                    }
                }

            }
            

                prevMove = plottedMove;
                if ((tile.position.x == targetTile.position.x && tile.position.y == targetTile.position.y) || tries > 1000)
                {
                running = false;
                if (tries > 1000)
                    Debug.Log("Tries Error");
                }

                else
                    tries++;


            
        }
    }

    void doorCheck(int xCoord, int yCoord, bool xDir)
    {
        RaycastHit2D hit;
        GameObject toInstantiate;
        for (int i = 0; i < roomBorders.Count; i++)
        {
            if (xCoord == roomBorders[i].x && yCoord == roomBorders[i].y)
            {
                toInstantiate = doors[Random.Range(0, doors.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(xCoord, yCoord, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
                instance.SendMessage("DoorSetup");
                if (xDir)
                {
                    hit = Physics2D.Linecast(new Vector2(xCoord, yCoord), new Vector2(xCoord, yCoord + 1), wallLayer);
                    if(hit.transform == null)
                    {
                        if (Random.Range(0, 4) == 0)
                        {
                            toInstantiate = wallTiles[0];
                        }
                        else
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(xCoord, yCoord + 1, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        wallUnits.Add(instance.transform);
                    }

                    hit = Physics2D.Linecast(new Vector2(xCoord, yCoord), new Vector2(xCoord, yCoord - 1), wallLayer);
                    if (hit.transform == null)
                    {
                        if (Random.Range(0, 4) == 0)
                        {
                            toInstantiate = wallTiles[0];
                        }
                        else
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(xCoord, yCoord - 1, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        wallUnits.Add(instance.transform);
                    }
                }
                else
                {
                    hit = Physics2D.Linecast(new Vector2(xCoord, yCoord), new Vector2(xCoord + 1, yCoord), wallLayer);
                    if (hit.transform == null)
                    {
                        if (Random.Range(0, 4) == 0)
                        {
                            toInstantiate = wallTiles[0];
                        }
                        else
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(xCoord + 1, yCoord, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        wallUnits.Add(instance.transform);
                    }

                    hit = Physics2D.Linecast(new Vector2(xCoord, yCoord), new Vector2(xCoord - 1, yCoord), wallLayer);
                    if (hit.transform == null)
                    {
                        if (Random.Range(0, 4) == 0)
                        {
                            toInstantiate = wallTiles[0];
                        }
                        else
                            toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                        instance = Instantiate(toInstantiate, new Vector3(xCoord - 1, yCoord, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(boardHolder);
                        wallUnits.Add(instance.transform);
                    }
                }
            }
        }
    }

    void addWalls()
    {
        //first go through the borders.
        RaycastHit2D hit;
        RaycastHit2D floorHit;
        GameObject toInstantiate;
        GameObject instance;
        for(int i = 0; i < roomBorders.Count; i++)
        {
            hit = Physics2D.Linecast(new Vector2(roomBorders[i].x, roomBorders[i].y), new Vector2(roomBorders[i].x + 0.25f, roomBorders[i].y), wallLayer);
            floorHit = Physics2D.Linecast(new Vector2(roomBorders[i].x, roomBorders[i].y), new Vector2(roomBorders[i].x + 0.25f, roomBorders[i].y), floorLayer);
            if (hit.transform == null && floorHit.transform == null)
            {
                if (Random.Range(0, 4) == 0)
                {
                    toInstantiate = wallTiles[0];
                }
                else
                    toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                instance = Instantiate(toInstantiate, new Vector3(roomBorders[i].x, roomBorders[i].y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
                wallUnits.Add(instance.transform);
            }
        }
        Vector2[] vecArray = { new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1) };
        for(int i = 0; i < hallTiles.Count; i++)
        {
            for(int x = 0; x < 8; x++)
            {
                hit = Physics2D.Linecast(new Vector2(hallTiles[i].position.x + vecArray[x].x, hallTiles[i].position.y + vecArray[x].y), new Vector2(hallTiles[i].position.x + vecArray[x].x + 0.25f, hallTiles[i].position.y + vecArray[x].y), wallLayer);
                floorHit = Physics2D.Linecast(new Vector2(hallTiles[i].position.x + vecArray[x].x, hallTiles[i].position.y + vecArray[x].y), new Vector2(hallTiles[i].position.x + vecArray[x].x + 0.25f, hallTiles[i].position.y + vecArray[x].y), floorLayer);
                if (hit.transform == null && floorHit.transform == null)
                {
                    if (Random.Range(0, 4) == 0)
                    {
                        toInstantiate = wallTiles[0];
                    }
                    else
                        toInstantiate = wallTiles[Random.Range(0, wallTiles.Length)];
                    instance = Instantiate(toInstantiate, new Vector3(hallTiles[i].position.x + vecArray[x].x, hallTiles[i].position.y + vecArray[x].y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                    wallUnits.Add(instance.transform);
                }
            }
        }
    }

    //void Update()
    //{

    //    if (Input.GetKeyDown(KeyCode.Space) && rooms.Count > 0)
    //    {
    //        GameObject hallTile = tileFloorTiles[Random.Range(0, tileFloorTiles.Length)];
    //        int startRoom = connectedRooms[Random.Range(0, connectedRooms.Count)];
    //        int endRand = Random.Range(0, rooms.Count);
    //        int endRoom = rooms[endRand];
    //        connectedRooms.Add(rooms[endRand]);
    //        rooms.RemoveAt(endRand);
    //        runHalls(startRoom, endRoom, hallTile);
    //    }
    //}
}






