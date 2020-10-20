using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool closed = true;
    private AudioSource openSound;
    private bool visible = false;
    private bool explored = false;
    public int tileSet = 0;

    public Sprite[] closedDownSprites;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    void Awake()
    {
        openSound = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DoorSetup()
    {
        tileSet = GameManager.instance.doorSet;
        spriteRenderer.sprite = closedDownSprites[tileSet];
        PathNode currNode = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        currNode.isOccupied = true;
        currNode.occupier = this.transform;
    }

    public void Open()
    {
        closed = false;
        spriteRenderer.sprite = null;   
        PathNode currNode = Pathfinding.Instance.GetNode(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        currNode.isOccupied = false;
        currNode.occupier = null;
        openSound.Play();
        this.enabled = false;

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
