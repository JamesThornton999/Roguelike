using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public bool enable = true;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.x = -10;
        currentPosition.y = -10;
        transform.position = currentPosition;
    }

    public void EnableDisable(bool input)
    {
        this.enabled = input;
        enable = input;
        if (enable == false)
        {
            Vector3 currentPosition = transform.position;
            currentPosition.x = -10;
            currentPosition.y = -10;
            transform.position = currentPosition;
        }

    }
}
