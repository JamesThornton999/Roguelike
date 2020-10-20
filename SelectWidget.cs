using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class SelectWidget : MonoBehaviour
{

    public SelectionUIOverlay selectionUIOverlay;

    private Image spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        selectionUIOverlay.Subscribe(this);
    }

    public void UpdateSprite(Sprite sprite)
    {
        if(sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }else
        {
            gameObject.SetActive(false);
        }

    }

}
