using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comment : MonoBehaviour
{
    Text comment;
    private float Interval = 2.0f;
    private float t = 0;
    bool change = false;

    public GameObject GameManagers;
    GameManager gameManager;
    void Start()
    {
        comment = GetComponent<Text>();
        gameManager = GameManagers.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!change)
        {
            if (t == 0)
            {
                if (gameManager.isPitcher)
                {
                    comment.text = "あなたはピッチャーです!";

                }
                else
                {
                    comment.text = "あなたはバッターです!";
                }
            }
            t += Time.deltaTime;
            if(t > Interval)
            {
                TextChange();
                change = true;
                t = 0;
            }
        }
    }

    private void TextChange()
    {
        if (gameManager.isPitcher)
        {
            comment.text = "ボールの軌道を描こう！";
        }
        else
        {
            comment.text = "タイミング良くボールを打とう！";
        }
        Invoke("ChangeBool", Interval);
    }

    void ChangeBool()
    {
        change = false;

    }
}
