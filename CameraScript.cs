using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    int xSize;
    int ySize;
    float xRadius = 11.24f;
    float yRadius = 5;
    float scrollSpeed = 8;

    private float cameraDistanceMax = 10f;
    private float cameraDistanceMin = 2f;
    private float cameraDistance = 5f;
    public float zoomSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        xSize = GameManager.instance.xSectors * GameManager.instance.sectorSize + 1;
        ySize = GameManager.instance.ySectors * GameManager.instance.sectorSize + 1;
        DetermineExtents();
    }

    private void DetermineExtents()
    {
        xRadius = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - transform.position.x;
        yRadius = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y - transform.position.y;
       // Debug.Log("X: " + xRadius + " Y: " + yRadius);
    }

    // Update is called once per frame
    void Update()
    {
        //Right
        if(Input.mousePosition.x > Screen.width * 0.95f)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed*2, Space.World);
            else
                transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.x < Screen.width * 0.05f)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed * 2, Space.World);
            else
                transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.y < Screen.height * 0.05f)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                transform.Translate(Vector3.down * Time.deltaTime * scrollSpeed * 2, Space.World);
            else
                transform.Translate(Vector3.down * Time.deltaTime * scrollSpeed, Space.World);
        }
        if (Input.mousePosition.y > Screen.height * 0.95f)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed * 2, Space.World);
            else
                transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed, Space.World);
        }
        
        Vector3 currentPosition = transform.position;
        //Debug.Log(Input.mouseScrollDelta.y);
        if (Input.mouseScrollDelta.y != 0)
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                cameraDistance += Input.mouseScrollDelta.y * scrollSpeed * 2 * Time.deltaTime * -1;
            else
                cameraDistance += Input.mouseScrollDelta.y * scrollSpeed  * Time.deltaTime * -1;
            cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
            DetermineExtents();
        }
        Camera.main.orthographicSize = cameraDistance;
        //Debug.Log(currentPosition.x + xRadius);
        if (currentPosition.x + xRadius > xSize)
        {
            currentPosition.x = xSize - xRadius;
        }else if(currentPosition.x - xRadius < -1)
            currentPosition.x = xRadius - 1;
        if (currentPosition.y + yRadius > ySize)
        {
            currentPosition.y = ySize - yRadius;
        }
        else if (currentPosition.y - yRadius < -6)
            currentPosition.y = yRadius - 6;
        transform.position = currentPosition;
    }
}
