using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [SerializeField]
    private int AnimationNum = 0;
    Vector2 Scale;
    Vector2 Pos;


    void Start()
    {
        Scale.x = this.gameObject.transform.localScale.x;
        Scale.y = this.gameObject.transform.localScale.y;
        Pos.x = this.transform.localPosition.x;
        Pos.y = this.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (AnimationNum == 0)
        {
            this.gameObject.transform.localScale = new Vector3(Scale.x + (Scale.x / 10 * Mathf.Sin(20 * Time.time)), Scale.y + (Scale.y / 10 * Mathf.Sin(20 * Time.time)), 1);
        }
        else if(AnimationNum == 1)
        {
            this.gameObject.transform.localPosition = new Vector3(Pos.x, Pos.y + (10* Mathf.Sin(5 * Time.time)), 0);
        }
    }
}
