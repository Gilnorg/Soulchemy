using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseItem : MonoBehaviour {

    private GameController gc;
    private RectTransform rTransform;
    private Image image;

	// Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        rTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        Item item = gc.currentItem;
        print(item);
		if (item != null)
        {
            rTransform.anchoredPosition = Input.mousePosition;
            if (image.sprite != item.sprite)
            {
                image.sprite = item.sprite;
            }
        }
        else
        {
            rTransform.anchoredPosition = Vector2.up;
        }
	}
}
