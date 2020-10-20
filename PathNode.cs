using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{

    private PlayGrid<PathNode> grid;
    public int x;
    public int y;
    public bool isWalkable = false;
    public bool isOccupied = false;
    public bool explored = false;
    public bool visible = false;

    public Transform occupier;
    public Transform doodad;
    public Transform tile;
    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;
    public PathNode(PlayGrid<PathNode> grid, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.grid = grid;
    }

    public void CalculateFCost()
    {
        this.fCost = gCost + hCost;
    }
    public override string ToString()
    {
        return x + ", " + y;
    }

    public void SetVisiblity(bool visible)
    {
        this.visible = visible;
        //Debug.Log(this.occupier);
        if (this.visible && !explored)
            explored = true;
        if (this.occupier != null && occupier.tag != "Heroes")
        {
           // Debug.Log(this.occupier.ToString());
            occupier.SendMessage("SetVisiblity", visible);
        }
        if(this.doodad != null)
        {
            //Debug.Log(this.doodad.ToString());
            doodad.SendMessage("SetVisiblity", visible);
        }
        if(this.tile != null)
        {
            tile.SendMessage("SetVisiblityNoUpdate", visible);
        }

    }
}
