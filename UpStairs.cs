using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpStairs : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite[] variants;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    public bool visible = false;
    public bool explored = false;
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PathNode node = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        node.doodad = this.transform;
    }

    public void updateTile(int style)
    {
        spriteRenderer.sprite = variants[style];
    }
    public void SetCollider(bool input)
    {
        boxCollider.enabled = input;
    }

    public void SetVisiblity(bool input)
    {
        if (input && !explored)
            explored = true;
        visible = input;
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
}
