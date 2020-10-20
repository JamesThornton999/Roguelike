using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public static Pathfinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private PlayGrid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new PlayGrid<PathNode>(width, height, 1, new Vector3(-0.5f, -0.5f, 0), (PlayGrid<PathNode> g, int x, int y) => new PathNode(g, x, y));

    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        if (startNode.isWalkable)
        {
            PathNode endNode = grid.GetGridObject(endX, endY);
            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    PathNode pathNode = grid.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();
            PathNode currentNode = startNode;
            while (openList.Count > 0)
            {
                currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    //Reached destination
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.isWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }

            }
        }
       
        //Out of nodes on the openList
        //return null;
        return CalculatePath(startNode);
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if(currentNode.x -1 >= 0)
        {
            //left
            neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0) //Left Down
                neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight()) //Left up
                neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y + 1));
        }
        if(currentNode.x + 1 < grid.GetWidth())
        {
            //right
            neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0) //Right Down
                neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight()) //Right up
                neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y + 1));
        }
        if (currentNode.y - 1 >= 0) //Down
            neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
        if (currentNode.y + 1 < grid.GetHeight()) //Up
            neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public List<Vector2> FindVector2Path(int startX, int startY, int endX, int endY)
    {
        List<PathNode> nodePath = FindPath(startX, startY, endX, endY);
        
        List<Vector2> vPath = new List<Vector2>();
        //Debug.Log("NodePathCount: " + nodePath.Count);
        for (int z = 1; z < nodePath.Count; z++)
        {
            Vector2 temp = new Vector2(nodePath[z].x, nodePath[z].y);
            vPath.Add(temp);
        }
        return vPath;


    }
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestCostNode.fCost)
                lowestCostNode = pathNodeList[i];
        }
        return lowestCostNode;
    }

    public bool SetOccupier(int x, int y, Transform input)
    {
        PathNode node = grid.GetGridObject(x, y);
        if (node.occupier == null)
        {
            node.occupier = input;
            return true;
        }
        else
            return false;
    }
    public void ClearOccupier(int x, int y)
    {
        PathNode node = grid.GetGridObject(x, y);
        node.occupier = null;
    }
    public void SetOccupied(int x, int y, bool value)
    {
        PathNode node = grid.GetGridObject(x, y);
        node.isOccupied = value;
    }
    public void SetWalkable(int x, int y, bool value)
    {
        PathNode node = grid.GetGridObject(x, y);
        node.isWalkable = value;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public void ClearVisiblity()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                if(pathNode != null)
                pathNode.SetVisiblity(false);
            }
        }
    }

    public Vector2 FindClosestWalkable(Vector2 input)
    {
        openList = new List<PathNode> { grid.GetGridObject(Mathf.FloorToInt(input.x), Mathf.FloorToInt(input.y))};
        closedList = new List<PathNode>();
        PathNode currentNode = openList[0];
        while(openList.Count > 0)
        {
            if (openList.Count > 0)
            {
                currentNode = openList[0];
                closedList.Add(currentNode);
                openList.Remove(currentNode);
            }

            foreach(PathNode selected in GetNeighbourList(currentNode))
            {
                if (!closedList.Contains(selected) && !openList.Contains(selected))
                    if (selected.isWalkable)
                    {
                        Debug.Log("Closest Movable Square: " + selected.x + ", " + selected.y);
                        return new Vector2(selected.x, selected.y);
                    }else
                    {
                        openList.Add(selected);
                    }

                
            }
        }
        return input;
    }
}
