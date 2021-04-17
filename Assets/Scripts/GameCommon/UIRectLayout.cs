using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRectLayout : MonoBehaviour
{
    public bool Reduce = false;

#if UNITY_IPHONE || UNITY_EDITOR
	void Awake ()
    {
        if (Screen.width == 2436 && Screen.height == 1125)
        {
            if(!Reduce)
            {
                AspectRatioFitter aspectRatioFitter = GetComponent<AspectRatioFitter>();
                if (aspectRatioFitter)
                {
                    aspectRatioFitter.aspectRatio = 2.165333f;
                }
                else
                {
                    RectTransform rectTransform = transform as RectTransform;
                    if (rectTransform.anchorMax.x == 1f)
                    {
                        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x - 44f, rectTransform.offsetMin.y);
                        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x + 44f, rectTransform.offsetMax.y);
                    }
                }
            }
            else
            {
                RectTransform rectTransform = transform as RectTransform;
                rectTransform.offsetMin = new Vector2(44f, 0f);
                rectTransform.offsetMax = new Vector2(-44f, 0f);
            }
        }
    }
#endif

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
