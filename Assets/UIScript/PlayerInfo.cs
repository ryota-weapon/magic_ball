
using UnityEngine;
using NCMB;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;


public class PlayerInfo : MonoBehaviour
{
    public Text Name;
    string BadName;
    string id;
    public Text AttentionText;
    public GameObject Attention;
    public GameObject  Button;
    bool switcha = true;
    bool Ok = true;
    void Start()
    {
       // PlayerPrefs.SetString("PlayerName", "");
        if (PlayerPrefs.GetString("PlayerName") != "")
        {
            Login();
            Load();          
        } 
    }

    // Update is called once per frame
    void Update()
    {

    }
     public void EnterNameToNCMB()  
    {
        if (Name.text == "") 
        {
        
            Attention.SetActive(true);
            AttentionText.text = "エラー：この名前は使えません。";
            return;
        }

        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerRate");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                return;
            }
            else
            {
                var names = objList.Select(o => System.Convert.ToString(o["Name"]));
                foreach (var item in names)
                {
                    if(Name.text == item)
                    {
                        print("sameName");
                        Attention.SetActive(true);
                        AttentionText.text = "エラー：同じ名前が存在しています。";
                        Ok = false;
                        BadName = Name.text;
                    }
                }
                if (Ok)
                {
                    switcha = false;
                }
                Ok = true;

            }

        });

        if (!switcha)
        {
            NCMBObject playerRate = new NCMBObject("PlayerRate");
            playerRate.SaveAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    //エラー処理
                }
                else
                {

                    //成功時の処理
                    playerRate["Name"] = Name.text;
                    playerRate["Rate"] = 1000;
                    id = playerRate.ObjectId;
                    playerRate.SaveAsync();
                    id = playerRate.ObjectId;
                    PlayerPrefs.SetString("PlayerName", Name.text);
                    PlayerPrefs.SetInt("PlayerRate", 1000);
                    PlayerPrefs.SetString("PlayerID", id);
                    PlayerPrefs.Save();
                    print(PlayerPrefs.GetString("PlayerID"));
                    ActiveFalse(Attention);
                    GotoTitle();
                    Button.SetActive(false);

                }
            });
        }
    }

    void Login()
    {
        NCMBObject LogIn = new NCMBObject("LogIn");
        LogIn.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
            }
            else
            {

                //成功時の処理
                LogIn["Text"] = System.DateTime.Now.Month.ToString() + "月" + System.DateTime.Now.Day.ToString()+ "日"
                + System.DateTime.Now.Hour.ToString() + ":" + System.DateTime.Now.Minute.ToString() + 
                "に"+PlayerPrefs.GetString("PlayerName") +"さんがゲームをプレイしています。";
                LogIn.SaveAsync();
            }
        });
    }
    public void SetPanel(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void ActiveFalse(GameObject a)
    {
        a.SetActive(false);
    }

    public void GotoTitle()
    {
        Login();
        Invoke("Load", 2.0f);
    }

   void Load()
    {

        SceneManager.LoadScene("TitleScene");
    }
}
