using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectUI : MonoBehaviour
{
    public List<Transform> selection;
    private List<Transform> selectionSquares;
    public GameObject selectionSquare;
    public bool leftClicked = false;
    public float leftClickedTime = 0f;
    public float dragTime = 0.1f;
    public SelectionUIOverlay selectionUi;

    void Awake()
    {
        selection = new List<Transform>();
        selectionSquares = new List<Transform>();
        for(int i = 0; i < 12; i++)
        {
            selectionSquares.Add(Instantiate(selectionSquare, new Vector3(0, 0, 0f), Quaternion.identity).transform);
            //Debug.Log(instance.ToString());
            //selectionSquares.Add(instance.transform);
            //instance.SendMessage("EnableDisable", false);
            selectionSquares[i].SendMessage("EnableDisable", false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !leftClicked)
        {
            leftClicked = true;
        }
        else if (Input.GetMouseButton(0) && leftClicked && leftClickedTime < dragTime)
        {
            leftClickedTime += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0) && leftClicked && leftClickedTime < dragTime)
        {
            leftClicked = false;
            leftClickedTime = 0;
            Debug.Log("Clicked: " + Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x) + ", " + Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
            Transform selected = SelectIndividual(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
            if (selected != null && Input.GetKey(KeyCode.LeftShift) && selection.Count < 12)
            {
                selection.Add(selected);
            }else if (selected != null)
            {
                selection.Clear();
                selection.Add(selected);
            }
            
        }else if(Input.GetMouseButtonUp(0))
        {
            leftClicked = false;
            leftClickedTime = 0;
        }
        SelectionBoxPlace();

        if(Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < selection.Count; i++)
            {
                if (selection[i].tag == "Heroes")
                    selection[i].SendMessage("RecieveNewQuedMoveOrder", new Vector2(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y)));
            }
        } else if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < selection.Count; i++)
            {
                if (selection[i].tag == "Heroes")
                    selection[i].SendMessage("RecieveNewMoveOrder", new Vector2(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y)));
            }
        }

        selectionUi.UpdateUI();
    }

    private void SelectionBoxPlace()
    {
        for(int i = 0; i < selectionSquares.Count; i++)
        {

            Vector3 currentPosition;
            if (i < selection.Count)
            {
                //selectionSquares[i].GetComponent<GameObject>().SetActive(true);
                //Debug.Log("Selection Outlined");
                currentPosition = selectionSquares[i].position;
                currentPosition.x = selection[i].position.x;
                currentPosition.y = selection[i].position.y;
                //Debug.Log("currentPosition: " + currentPosition.x + ", " + currentPosition.y);
            }
            else
            {
                currentPosition = selectionSquares[i].position;
                currentPosition.x = -10;
                currentPosition.y = -10;
            }

            selectionSquares[i].position = currentPosition;

            //Debug.Log("Position: " + selectionSquares[i].position.x + ", " + selectionSquares[i].position.y);
               // selectionSquares[i].GetComponent<GameObject>().SetActive(false);

        }
    }
    public Transform SelectIndividual(int x, int y)
    {
        Transform selected;
        if (Pathfinding.Instance.GetNode(x, y).occupier != null && Pathfinding.Instance.GetNode(x, y).isOccupied)
            selected = Pathfinding.Instance.GetNode(x, y).occupier;
        else
            selected = null;
        return selected;
    }

}
