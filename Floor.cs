using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour
{
    public Sprite standard;
    public Sprite[] variants;

    private int variant = 0;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public bool visible = false;
    public bool explored = false;
    // Start is called before the first frame update
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        int Rand = Random.Range(0, 2);
        if (Rand == 1)
        {
            variant = Random.Range(0, variants.Length);
            spriteRenderer.sprite = variants[variant];
        }
        else
            variant = 0;
    }

    public void SetVisiblity(bool input)
    {
        visible = input;
        if (visible && !explored)
            explored = true;
        PathNode path = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        path.SetVisiblity(visible);
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
        if(!explored)
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
    public void SetColliderOff()
    {
        boxCollider.enabled = false;
    }

    public void SetColliderOn()
    {
        boxCollider.enabled = true;
    }

    public void UpdateLayer(int layer)
    {
        gameObject.layer = layer;
        SetVisiblity(false);
    }
}
