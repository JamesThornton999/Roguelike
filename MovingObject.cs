using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public LayerMask losLayer;
    public List<Vector2> moveList;
    public float moveSpeed = 0.5f;
    public bool frame = false;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private float inverseMoveTime;
    public Vector2 movingTo;
    public Vector2 moveTarget;
    public Vector2 finalMoveTarget;

    public bool visible = false;
    public bool moving = false;
    public bool acting = false;

    protected float actionTimer = 0f;
    protected float actionTimerStart = 0f;

    private float HP;
    private float MP;

    private int str;
    private int end;
    private int dex;
    private int agi;
    private int wis;
    private int wil;

    private int armour;
    private int encumberance;


    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inverseMoveTime = 1f / moveSpeed;
        movingTo = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        path.occupier = this.transform;
        path.isOccupied = true;
        moveTarget = movingTo;
        finalMoveTarget = movingTo;
        Vector3 currentPosition = this.transform.position;
        currentPosition.x = movingTo.x;
        currentPosition.y = movingTo.y;
        this.transform.position = currentPosition;
    }

    protected void DetermineDirection(Vector2 target,out int xMove, out int yMove)
    {
        if (transform.position.x < target.x) xMove = 1;
        else if (transform.position.x > target.x) xMove = -1;
        else xMove = 0;

        if (transform.position.y < target.y) yMove = 1;
        else if (transform.position.y > target.y) yMove = -1;
        else yMove = 0;
    }

    protected void StartMove(Vector2 target)
    {
       PathNode currNode = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
       PathNode targetNode = Pathfinding.Instance.GetNode(Mathf.FloorToInt(target.x), Mathf.FloorToInt(target.y));
        targetNode.isOccupied = true;
        targetNode.occupier = this.transform;
        currNode.isOccupied = false;
        currNode.occupier = null;
        movingTo = target;
        moving = true;
        CalculateLineOfSight();
        MoveCycle();
    }
    protected bool AttemptMove(Vector2 target, out int xMove, out int yMove)
    {
        
        if(movingTo == moveTarget)
        {
            xMove = 0;
            yMove = 0;
            return true;
        }

        DetermineDirection(target, out xMove, out yMove);

        PathNode node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
        //Debug.Log("Checked: " + (Mathf.FloorToInt(transform.position.x) + xMove) + ", " + (Mathf.FloorToInt(transform.position.y) + yMove));
        if (node.isOccupied == true || node.isWalkable == false)
        {
            //Debug.Log("Tile Occupied/ Unwalkable; " + xMove + ", " + yMove);
            //logic to choose the next trial tile
            if (!AttemptInteraction(xMove, yMove))
            {
                if (xMove != 0 && yMove != 0)
                {
                    //Diagonal Logic
                    if (frame)
                    {
                        xMove = 0;
                        node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                        //Debug.Log("Checked: " + (Mathf.FloorToInt(transform.position.x) + xMove) + ", " + (Mathf.FloorToInt(transform.position.y) + yMove));
                        if (node.isOccupied == true || node.isWalkable == false)
                        {
                            //Debug.Log("Tile Occupied/ Unwalkable; 2");
                            if (!AttemptInteraction(xMove, yMove))
                            {
                                DetermineDirection(target, out xMove, out yMove);
                                yMove = 0;
                                node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                //Debug.Log("Checked: " + (Mathf.FloorToInt(transform.position.x) + xMove) + ", " + (Mathf.FloorToInt(transform.position.y) + yMove));
                                if (node.isOccupied == true || node.isWalkable == false)
                                {
                                    if (!AttemptInteraction(xMove, yMove))
                                    {
                                       // Debug.Log("Tile Occupied/ Unwalkable; 3");
                                        return false;
                                    }
                                    else
                                        return false;

                                }
                                else
                                    return true;
                            }
                            else
                                return true;
                           
                        }
                        else
                            return true;
                    }
                    else
                    {
                        yMove = 0;
                        node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                        //Debug.Log("Checked: " + (Mathf.FloorToInt(transform.position.x) + xMove) + ", " + (Mathf.FloorToInt(transform.position.y) + yMove));
                        if (node.isOccupied == true || node.isWalkable == false)
                        {
                            if (!AttemptInteraction(xMove, yMove))
                            {
                              //  Debug.Log("Tile Occupied/ Unwalkable; 2");
                                DetermineDirection(target, out xMove, out yMove);
                                xMove = 0;
                                node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                //Debug.Log("Checked: " + (Mathf.FloorToInt(transform.position.x) + xMove) + ", " + (Mathf.FloorToInt(transform.position.y) + yMove));
                                if (node.isOccupied == true || node.isWalkable == false)
                                {
                                    //Debug.Log("Tile Occupied/ Unwalkable; 3");
                                    return false;
                                }
                                else
                                    return true;
                            }
                            else
                                return false;
                            
                        }
                        else
                            return true;
                    }
                }
                else
                {
                    //horizontal Logic
                    if (xMove == 0)
                    {
                        if (frame)
                        {
                            xMove = 1;
                            node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                            if (node.isOccupied == true || node.isWalkable == false)
                            {
                                if (!AttemptInteraction(xMove, yMove))
                                {
                                    //Debug.Log("Tile Occupied/ Unwalkable; 2");
                                    DetermineDirection(target, out xMove, out yMove);
                                    xMove = -1;
                                    node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                    if (node.isOccupied == true || node.isWalkable == false)
                                    {
                                        AttemptInteraction(xMove, yMove);
                                        DetermineDirection(target, out xMove, out yMove);
                                        //Debug.Log("Tile Occupied/ Unwalkable; 3");
                                        return false;
                                    }
                                    else
                                        return true;
                                }
                                else
                                    return false;
                                
                            }
                            else
                                return true;
                        }
                        else
                        {
                            xMove = -1;
                            node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                            if (node.isOccupied == true || node.isWalkable == false)
                            {
                                if (!AttemptInteraction(xMove, yMove))
                                {
                                    //Debug.Log("Tile Occupied/ Unwalkable; 2");
                                    DetermineDirection(target, out xMove, out yMove);
                                    xMove = 1;
                                    node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                    if (node.isOccupied == true || node.isWalkable == false)
                                    {
                                        AttemptInteraction(xMove, yMove);
                                        DetermineDirection(target, out xMove, out yMove);
                                        //Debug.Log("Tile Occupied/ Unwalkable; 3");
                                        return false;
                                    }
                                    else
                                        return true;
                                }
                                else
                                    return false;
                            }
                            else
                                return true;
                        }
                    }
                    else
                    {
                        if (frame)
                        {
                            yMove = 1;
                            node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                            if (node.isOccupied == true || node.isWalkable == false)
                            {
                                if (!AttemptInteraction(xMove, yMove))
                                {
                                    //Debug.Log("Tile Occupied/ Unwalkable; 2");
                                    DetermineDirection(target, out xMove, out yMove);
                                    yMove = -1;
                                    node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                    if (node.isOccupied == true || node.isWalkable == false)
                                    {
                                        AttemptInteraction(xMove, yMove);
                                        DetermineDirection(target, out xMove, out yMove);
                                        //Debug.Log("Tile Occupied/ Unwalkable; 3");
                                        return false;
                                    }
                                    else
                                        return true;
                                }
                                else
                                    return false;

                            }
                            else
                                return true;
                        }
                        else
                        {
                            yMove = -1;
                            node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                            if (node.isOccupied == true || node.isWalkable == false)
                            {
                                //Debug.Log("Tile Occupied/ Unwalkable; 2");
                                if (!AttemptInteraction(xMove, yMove))
                                {
                                    DetermineDirection(target, out xMove, out yMove);
                                    yMove = 1;
                                    node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x) + xMove, Mathf.FloorToInt(transform.position.y) + yMove);
                                    if (node.isOccupied == true || node.isWalkable == false)
                                    {
                                        AttemptInteraction(xMove, yMove);
                                        //Debug.Log("Tile Occupied/ Unwalkable; 3");
                                        DetermineDirection(target, out xMove, out yMove);
                                        return false;
                                    }
                                    else
                                        return true;
                                }
                                else
                                    return false;

                            }
                            else
                                return true;
                        }
                    }

                }
            }
            else
                return false;
           
        }else
        {
            return true;
        }

    }
    protected void MoveCycle()
    {
        int xMove;
        int yMove;
        DetermineDirection(movingTo, out xMove, out yMove);
        Vector3 currentPosition = transform.position;
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        float distance = Vector2.Distance(myPos, movingTo);
        if (moveSpeed * Time.deltaTime > distance)
        {
            //Debug.Log("I have arrived at " + movingTo.x + ", " + movingTo.y);
            currentPosition.x = movingTo.x;
            currentPosition.y = movingTo.y;
            moving = false;
        }else
        {
            currentPosition.x = currentPosition.x + (moveSpeed * xMove * Time.deltaTime);
            currentPosition.y = currentPosition.y + (moveSpeed * yMove * Time.deltaTime);
        }

        transform.position = currentPosition;
    }

    public abstract bool AttemptInteraction(int xMove, int yMove);

    public abstract void CalculateLineOfSight();
    protected void UpdateCycle()
    {
        frame = !frame;
        if (acting)
        {
            //Debug.Log("Acting");
            Vector3 currentPosition = transform.position;
            if (actionTimer < Time.deltaTime)
            {
                actionTimer = 0;
                actionTimerStart = 0;
                currentPosition.x = movingTo.x;
                currentPosition.y = movingTo.y;
                acting = false;
            }
            else
            {
                actionTimer -= Time.deltaTime;
                int xMove = 0;
                int yMove = 0;
                DetermineDirection(movingTo, out xMove, out yMove);
                //Debug.Log("Move: " + (0.5f * (1 - (actionTimer / actionTimerStart))));
                currentPosition = Vector3.MoveTowards(currentPosition, movingTo, 0.5f * (1 - (actionTimer / actionTimerStart)));
                actionTimerStart = actionTimer;
            }
            transform.position = currentPosition;
        }
        else if (transform.position.x != finalMoveTarget.x || transform.position.y != finalMoveTarget.y)
        {
            if (!moving)
            {
                if ((transform.position.x == moveTarget.x && transform.position.y == moveTarget.y) && moveList.Count > 0)
                {
                    //Debug.Log("Selecting the next node");
                    moveTarget = moveList[0];
                    moveList.RemoveAt(0);
                }

                int xMove = 0;
                int yMove = 0;
                //Debug.Log("Attempting to Move. MyPos: " + transform.position.x + ", " + transform.position.y + " movingTo " + moveTarget.x + ", " + moveTarget.y);
                if (AttemptMove(moveTarget, out xMove, out yMove))
                {
                    movingTo = new Vector2(transform.position.x + xMove, transform.position.y + yMove);
                    //Debug.Log("New Target Square: " + movingTo.x + ", " + movingTo.y);
                    //movingTo = new Vector2((int)(transform.position.x + xMove), (int)(transform.position.y + yMove));
                    //movingTo = new Vector2((int)(moveTarget.x), (int)(moveTarget.y));
                    StartMove(movingTo);
                }
                else if (!acting)
                {
                    moveList.Clear();
                    finalMoveTarget = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
                }
            }
            else
            {
                MoveCycle();
            }
        }
    }

    protected void ActTowards(Vector2 direction, float coolDown)
    {
        acting = true;
        actionTimerStart = coolDown;
        actionTimer = coolDown;
        movingTo = new Vector2(transform.position.x, transform.position.y);
        Vector3 currentPosition = this.transform.position;
        currentPosition.x += direction.x / 2;
        currentPosition.y += direction.y / 2;
        transform.position = currentPosition;
    }

    public void SetVisibilty(bool input)
    {
        visible = input;
        if (visible)
        {
            spriteRenderer.color = Color.white;
        }
        else
            spriteRenderer.color = Color.black;
    }

    public Vector2 GetDirectionVector2D(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

}
