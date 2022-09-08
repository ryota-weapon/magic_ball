using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class TitleSceneScript : MonoBehaviour
{
    public Text Name;
    public Text NameGene;
    public RectTransform TextPos;
    public RectTransform TextPos2;
    string BeforeRate, BeforeRate2;
    int BeforeRank, BeforeRank2;
    Text[] Texts = new Text[10];
    Text[] Rivals = new Text[10];
    public Text[] LogIns = new Text[10];
    public GameObject canvas;
    int i, j = 0;
    int indexcount;
    bool RateisSame = false;
    bool NameisSame = false;
    int IndexNum = 0;
    int MyRank = 0;

    bool isSecond, isSecond2 = false;



    void Start()
    {
        NCMBObject obj2 = new NCMBObject("PlayerRate");
        obj2.ObjectId = PlayerPrefs.GetString("PlayerID");
        print(PlayerPrefs.GetString("PlayerID"));
        obj2.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
                print("データ取得失敗");
            }
            else
            {
                //成功時の処
                var Rate = obj2["Rate"];
                print(obj2["Name"]);
                Name.text = PlayerPrefs.GetString("PlayerName") + ", レート: " + Rate;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GotoOnline()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GotoNotOnline()
    {
        SceneManager.LoadScene("GameSceneNotOnline");
    }

    public void GotoNotBattingPractice()
    {
        SceneManager.LoadScene("GameSceneAI");
    }


    public void PanleAppear(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void PanelDelete(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0, 1350);
    }

    public void Serch()
    {
        if (isSecond2)
        {
            return;
        }
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {
                var rates = objList.Select(o => System.Convert.ToInt32(o["Rate"]));
                var names = objList.Select(o => System.Convert.ToString(o["Name"]));

                foreach (var item in rates)
                {
                    Debug.Log(": " + item);

                    if (i > 9)
                    {
                        continue;
                    }
                    float Bias = 50f;
                    Text UI = Instantiate(NameGene, new Vector2(TextPos.anchoredPosition.x, TextPos.anchoredPosition.y - (Bias * i)), Quaternion.identity);
                    UI.text = item + "";
                    i++;
                    Texts[i - 1] = UI;
                    if (item == PlayerPrefs.GetInt("PlayerRate"))
                    {
                        RateisSame = true;
                        IndexNum = i - 1;
                    }

                }
                for (int n = i; n < 10; n++)
                {
                    float Bias = 50f;
                    Text UI = Instantiate(NameGene, new Vector2(TextPos.anchoredPosition.x, TextPos.anchoredPosition.y - (Bias * n)), Quaternion.identity);
                    UI.text = "---";
                    Texts[n] = UI;
                    Texts[n].transform.SetParent(this.canvas.transform, false);
                }
                i = 0;
                foreach (var item in names)
                {
                    if (i > 9)
                    {
                        continue;
                    }
                    // Debug.Log(": " + item);
                    string Rate = Texts[i].text;
                    if (Rate == BeforeRate) {
                        Texts[i].text = BeforeRank + "位:" + item + ":" + Rate;
                    }
                    else
                    {
                        Texts[i].text = i + 1 + "位:" + item + ":" + Rate;
                        BeforeRank = i + 1;
                    }
                    BeforeRate = Rate;
                    Texts[i].transform.SetParent(this.canvas.transform, false);
                    if (item == PlayerPrefs.GetString("PlayerName"))
                    {
                        Texts[i].color = Color.red;
                        RateisSame = false;
                        IndexNum = 0;
                    }
                    i++;
                }
                isSecond2 = true;

            }
        });

    }

    public void SerchNearRate()
    {
        if (isSecond)
        {
            return;
        }
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.OrderByDescending("Rate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {
                indexcount = 0;
                var rates = objList.Select(o => System.Convert.ToInt32(o["Rate"]));
                var names = objList.Select(o => System.Convert.ToString(o["Name"]));

                i = 0;
                foreach (var item in names)
                {
                    i++;
                    if (PlayerPrefs.GetString("PlayerName") == item)
                    {
                        print("上から" + i + "番目にいる");
                        MyRank = i;
                    }

                }

                int MaxLength = 4;
                int MaxInvest = MaxLength + MyRank;
                bool Stop = false;
                int StopNum = 0;
                int NewRate = 0;
                int NewRank = 0;
                int CopyNewRank = 0;
                int[] Ranks = new int[9];
                int BeforeRating = 0;
                foreach (var item in rates)
                {
                    j++;
                    if (NewRate == item)
                    {

                    }
                    else
                    {
                        NewRank++;
                    }
                    NewRate = item;
                    if (Mathf.Abs(MyRank - j) > MaxLength)
                    {

                    }
                    else
                    {
                        if (!Stop)
                        {
                            StopNum = j; //j番目からランキング表示
                            CopyNewRank = NewRank;
                            Ranks[indexcount] = NewRank;
                            Stop = true;
                        }
                        if (BeforeRating == item)
                        {
                            Ranks[indexcount] = Ranks[indexcount - 1];
                        }
                        else if(indexcount != 0)
                        {
                            Ranks[indexcount] = Ranks[indexcount - 1] + 1;
                        }
                        float Bias = 50f;
                        Text UI = Instantiate(NameGene, new Vector2(TextPos2.anchoredPosition.x, TextPos2.anchoredPosition.y - (Bias * indexcount)), Quaternion.identity);
                        UI.text = item + "";
                        Rivals[indexcount] = UI;
                        Rivals[indexcount].transform.SetParent(this.canvas.transform, false);
                        print("生成！");
                        indexcount++;
                        BeforeRating = item;
                        if (indexcount >= MaxInvest)
                        {
                            continue;
                        }
                    }
                }
                int MaxCount = MaxInvest; ;
                print(StopNum + "StopNUm");
                i = 0;
                indexcount = 0;
                foreach (var item in names)
                {

                    i++;

                    if (i >= StopNum)
                    {
                        if (indexcount >= 9)
                        {
                            continue;
                        }
                        string Rate = Rivals[indexcount].text;
                       // print(Rate);
                        if (Rate == BeforeRate2)
                        {

                            if (PlayerPrefs.GetString("PlayerName") == item)
                            {
                                Rivals[indexcount].color = Color.red;
                            }
                            Rivals[indexcount].text = (Ranks[indexcount]) + "位:" + item + ":" + Rate;
                            print(Rivals[indexcount].text);
                        }
                        else
                        {

                            if (PlayerPrefs.GetString("PlayerName") == item)
                            {
                                Rivals[indexcount].color = Color.red;
                            }
                            Rivals[indexcount].text = (Ranks[indexcount]) + "位:" + item + ":" + Rate;
                            BeforeRank2 = i;
                            print(Rivals[indexcount].text);
                        }
                        BeforeRate2 = Rate;

                        Rivals[indexcount].transform.SetParent(this.canvas.transform, false);
                        indexcount++;
                    }

                }
                isSecond = true;

            }

        });
    }

    public void SerchLogIns()
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("LogIn");
        query.OrderByDescending("updateDate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogWarning("取得に失敗: " + e.ErrorMessage);
            }
            else
            {            
                var Text = objList.Select(o => System.Convert.ToString(o["Text"]));
                int i = 0;
                foreach(var item in Text)
                {
                    if(i >= 9 && item == null)
                    {
                        break;
                    }
                    LogIns[i].text = item;
                    i++;
                }
            }
        });



    }
}
