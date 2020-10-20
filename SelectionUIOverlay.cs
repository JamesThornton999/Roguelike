using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUIOverlay : MonoBehaviour
{
    public List<SelectWidget> selectWidgets;
    public SelectUI selectUI;

    public void Awake()
    {
        selectWidgets = new List<SelectWidget>();
    }

    public void Subscribe(SelectWidget widget)
    {
        selectWidgets.Add(widget);
    }
    public void UpdateUI()
    {
        for(int i = 0; i < selectWidgets.Count; i++)
        {
            SpriteRenderer temp = selectUI.selection[i].gameObject.GetComponent<SpriteRenderer>();
            if(i < selectWidgets.Count)
                selectWidgets[i].UpdateSprite(temp.sprite);
        }
        //for(int i = 0; i < selection.Count; i++)
        //{

        //    if (temp != null)
        //        selectWidgets[i].UpdateSprite(temp.sprite);
        //    else
        //        selectWidgets[i].gameObject.SetActive(false);
        //}
    }


}
