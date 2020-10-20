using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    private const int LOS_RAYCASTS = 360;
    private const float LOS_DISTANCE = 10;
    public override void CalculateLineOfSight()
    {
        float angle = 0;
        Pathfinding.Instance.ClearVisiblity();
        List<Transform> oldHits = new List<Transform>();
        for(int i = 0; i < LOS_RAYCASTS; i++)
        {

            angle = (360 / LOS_RAYCASTS) * i;
            RaycastHit2D[] hits = Physics2D.RaycastAll(movingTo, GetDirectionVector2D(angle), LOS_DISTANCE, losLayer,-10,10);
            //Debug.Log("LOScalcs: " + hits.Length);
            for (int x = 0; x < hits.Length; x++)
            {
                //Debug.Log(hits[x].ToString());
                if (!oldHits.Contains(hits[x].transform))
                {
                    if (hits[x].transform.tag == "Wall")
                    {
                        hits[x].transform.SendMessage("SetVisiblity", true);
                        break;
                    }
                    else
                    {
                        hits[x].transform.SendMessage("SetVisiblity", true);
                        PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(hits[x].transform.position.x), Mathf.FloorToInt(hits[x].transform.position.y));
                        if (path.occupier != null)
                            if (path.occupier.tag == "Door" && GameManager.instance.doorsBlockLOS)
                                break;
                            else
                                oldHits.Add(hits[x].transform);
                    }
                }
                //else
                    //Debug.Log("Im already checked!");
                  
            }
        }
    }
    public override bool AttemptInteraction(int xMove, int yMove)
    {
        PathNode node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
        Transform obj = node.occupier;
        if (obj != null)
            if (obj.tag == "Door")
            {
                obj.SendMessage("Open");
                ActTowards(new Vector2(xMove, yMove), 1.0f);
                CalculateLineOfSight();
                return true;
            }
        return false;
    }

    public void RecieveNewMoveOrder(Vector2 destination)
    {

        PathNode node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.y));
        if(node.isWalkable)
        {
            moveList.Clear();
            moveList = Pathfinding.Instance.FindVector2Path(Mathf.FloorToInt(movingTo.x), Mathf.FloorToInt(movingTo.y), Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.y));
           // Debug.Log("Movelist: " + moveList.Count);

        }else
        {
            moveList.Clear();
            Vector2 altSquare = Pathfinding.Instance.FindClosestWalkable(destination);
            moveList = Pathfinding.Instance.FindVector2Path(Mathf.FloorToInt(movingTo.x), Mathf.FloorToInt(movingTo.y), Mathf.FloorToInt(altSquare.x), Mathf.FloorToInt(altSquare.y));
        }
        if (moveList.Count > 0)
            finalMoveTarget = moveList[moveList.Count - 1];
        else
            finalMoveTarget = movingTo;
    }

    public void RecieveNewQuedMoveOrder(Vector2 destination)
    {
        Vector2 start;
        if(moveList.Count > 0)
        {
            start = moveList[moveList.Count - 1];
        }else
        {
            start = movingTo;
        }
        PathNode node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.y));
        List<Vector2> quedPath = new List<Vector2>();
        if (node.isWalkable)
        {
        quedPath = Pathfinding.Instance.FindVector2Path(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y), Mathf.FloorToInt(destination.x), Mathf.FloorToInt(destination.y));
        }else
        {
            Vector2 altSquare = Pathfinding.Instance.FindClosestWalkable(destination);
            quedPath = Pathfinding.Instance.FindVector2Path(Mathf.FloorToInt(movingTo.x), Mathf.FloorToInt(movingTo.y), Mathf.FloorToInt(altSquare.x), Mathf.FloorToInt(altSquare.y));
        }
           
        for(int i = 0; i < quedPath.Count; i++)
        {
            moveList.Add(quedPath[i]);
        }
        if (moveList.Count > 0)
            finalMoveTarget = moveList[moveList.Count - 1];
        else
            finalMoveTarget = movingTo;
    }

    protected override void Start()
    {
        //GameObject downStairs = GameObject.FindGameObjectWithTag("DownStairs");
        //moveList = Pathfinding.Instance.FindVector2Path(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y),
        //    Mathf.FloorToInt(downStairs.transform.position.x), Mathf.FloorToInt(downStairs.transform.position.y));
        //finalMoveTarget = new Vector2(Mathf.FloorToInt(downStairs.transform.position.x), Mathf.FloorToInt(downStairs.transform.position.y));
        base.Start();
        CalculateLineOfSight();
    }
    void Update()
    {
        UpdateCycle();

    }
        
    
}
