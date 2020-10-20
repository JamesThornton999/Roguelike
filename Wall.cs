using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public LayerMask wallLayer;
    public Sprite[] regWall;
    public Sprite[] upWall;
    private int Type = 0;

    private bool visible = false;
    private bool explored = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void SetVisiblity(bool input)
    {
        visible = input;
        if (visible && !explored)
            explored = true;
        PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));


        UpdateVisiblity();
    }

    public void SetVisiblityNoUpdate(bool input)
    {
        visible = input;
        if (visible && !explored)
            explored = true;
        //PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        //path.SetVisiblity(visible);
        UpdateVisiblity();
    }
    private void UpdateVisiblity()
    {
        if (!explored)
        {
            spriteRenderer.color = Color.black;
        }
        else if (visible)
        {
            spriteRenderer.color = Color.white;
        }
        else if (explored && !visible)
            spriteRenderer.color = Color.grey;
        
    }
    //This next method will raycast down 1 unit. If it finds a wall, it will change to the appropriate regWall tile. Else, it will switch to the proper upWall graphic.
    public void changeTile(int wallType)
    {
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y -1), wallLayer);
        if (hit.transform == null || hit.collider.gameObject.tag == "Door")
        {
            spriteRenderer.sprite = upWall[wallType];
        }else
        {
            spriteRenderer.sprite = regWall[wallType];
        }
        boxCollider.enabled = true;

    }

    public void UpdateLayer(int input)
    {
        gameObject.layer = input;
        SetVisiblity(false);
    }
}
