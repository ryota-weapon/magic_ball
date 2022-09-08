using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pencile : MonoBehaviour
{
    public RectTransform rect;
    RectTransform myRect;
    void Start()
    {
        myRect = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        myRect.transform.position = rect.transform.position;
    }
}
