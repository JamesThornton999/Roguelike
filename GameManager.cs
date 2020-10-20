using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public static GameManager instance = null;
    public int tileSet = 0;
    public int doorSet = 0;
    public int xSectors = 5;
    public int ySectors = 5;
    public int sectorSize = 14;
    public BoardGenerator boardScript;
    public Pathfinding pathfinding;
    private bool doingSetup = false;
    public bool doorsBlockLOS = true;
    
    // Start is called before the first frame update
    private void Awake()
    {

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        //grid = new Grid<int>(xSectors * sectorSize, ySectors * sectorSizes) ;
        boardScript = GetComponent<BoardGenerator>();
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;
        tileSet = Random.Range(0, 4);
        doorSet = Random.Range(0, 4);
        pathfinding = new Pathfinding(xSectors * sectorSize, ySectors * sectorSize);
        boardScript.prepBorders();
        boardScript.BoardSetup(tileSet);
        doingSetup = false;
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        for(int i = 0; i < floors.Length; i++)
        {
            setPathing((int)floors[i].transform.position.x, (int)floors[i].transform.position.y, true);
        }
        GameObject upStairs = GameObject.FindGameObjectWithTag("UpStairs");
        GameObject instance = Instantiate(player, new Vector3(upStairs.transform.position.x, upStairs.transform.position.y, 0f), Quaternion.identity) as GameObject;
        Camera.main.transform.position = new Vector3(instance.transform.position.x, instance.transform.position.y, Camera.main.transform.position.z);
    }

    public void setPathing(int x, int y, bool value)
    {
        pathfinding.SetWalkable(x, y, value);
    }
    // Update is called once per frame

}
