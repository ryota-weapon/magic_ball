using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManagerNotOnline : MonoBehaviour
{
    public GameObject Ads;
    GoogleAds GoogleAdsScript;

    public Text PitcherOrBatter;
    public Text IsBatterReadly;
    public Text CountBall;
    public Text Ining;
    public Text Player1sPoint;
    public Text Player2sPoint;
    public Text Winner;

    LineRenderer lineRenderer;
    private bool isShowGoSignImage = false;

    public RectTransform IsBatterReady;
    public RectTransform Change;
    public RectTransform SwingButton;
    public RectTransform GameSetPanel;

    AudioSource audioSource;
    public AudioClip Syoubu_Sound;
    public AudioClip StepSound;
    public AudioClip StartSound;
    public AudioClip GameSetSound;
    public RectTransform GoSign;
    private Vector2 StartPos_Of_Image = new Vector2(-1320f, 0);
    private Vector2 EndPos_Of_Image = new Vector2(0, 0);

    bool oneshot = false;

    public Image CanvasPanel;
    public Image StrikeZonePanel;
    public Image PitcherZonePanel;

    public GameObject Pitcher;
    PitcherScript pitcherScript;

    public GameObject Batter;
    BatterScript batterScript;

    [HideInInspector]
    public bool Player1isPitcher = false;

    [HideInInspector]
    public Vector2 PlayerPoints = Vector2.zero;

    [HideInInspector]
    public bool NextIningIsLast = false;

    [HideInInspector]
    public int count = 0;
    int IningCount = 0;



    float delayTime = 0.5f;
    void Start()
    {
        Player1isPitcher = true;
        PitcherOrBatter.text = "プレイヤー1はボールの軌道を描いてください";
        IsBatterReadly.text = "プレイヤー2に端末を渡してください";
        audioSource = GetComponent<AudioSource>();
        pitcherScript = Pitcher.GetComponent<PitcherScript>();
        batterScript = Batter.GetComponent<BatterScript>();
        GoogleAdsScript = Ads.GetComponent<GoogleAds>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isShowGoSignImage)
        {

            GoSign.anchoredPosition = Vector2.Lerp(GoSign.anchoredPosition, EndPos_Of_Image, 0.1f);
            if (EndPos_Of_Image.x - GoSign.anchoredPosition.x < 1.0f && !oneshot)
            {
                oneshot = true;

                Invoke("ShowGoSignImage", delayTime);
            }
        }
        else
        {
            GoSign.anchoredPosition = Vector2.Lerp(GoSign.anchoredPosition, StartPos_Of_Image, 0.1f);
            if (GoSign.anchoredPosition.x - StartPos_Of_Image.x < 1.0f && oneshot)
            {
                oneshot = false;
                Invoke("BeReadyToThrow", delayTime);

            }
        }
    }


    public void WhenLineMade()
    {
        lineRenderer = GameObject.FindWithTag("SendLine").GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
        PanelAppear(IsBatterReady);
    }
    public void ShowGoSignImage()
    {
        if (!isShowGoSignImage)
        {
            ChangeAlfa(false);
            isShowGoSignImage = true;
            audioSource.PlayOneShot(Syoubu_Sound);
        }
        else
        {
            isShowGoSignImage = false;
        }

    }

    private void BeReadyToThrow()
    {
        pitcherScript.StartThrow();
        isShowGoSignImage = false;
        batterScript.animator.SetTrigger("Ready");
        batterScript.StartSimulation = true;
        batterScript.Simulation_SwingTime = 0;
        audioSource.PlayOneShot(StepSound);
        SwingButton.anchoredPosition = new Vector2(0, -260f);
        if(IningCount == 3)
        {
            NextIningIsLast = true;
        }
    }

    public void SetPitcherNormalPosition()
    {
 
        count++;
        pitcherScript.animator.SetTrigger("Back");
        batterScript.animator.SetTrigger("Back");
        batterScript.Simulation_SwingTime = 0;
        GameObject touchField = GameObject.FindWithTag("Canvas");
        TouchField touchFieldScript = touchField.GetComponent<TouchField>();
        touchFieldScript.DeleteLine();


        if (count == 3)
        {
            IningCount++;
            if(IningCount >= 4)
            {
                GameSet();
                return;
            }

            if (Player1isPitcher)
            {
                Player1isPitcher = false;
            }
            else
            {
                Player1isPitcher = true;
            }
            Invoke("ChangePanelDelete", 1.0f);
            IningShow();
            count = 0;
        }
        CountBall.text = (count + 1) + "球目";
        ChangeAlfa(true);
        ChangeText(false);
    }

    public void PanelAppear(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void PanelDelete(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0,1600);
    }

    void ChangePanelDelete()
    {
        PanelDelete(Change);
        audioSource.PlayOneShot(StartSound);
    }

    private void ChangeAlfa(bool a)
    {
        if (a)
        {
            var C_C = CanvasPanel.color;
            CanvasPanel.color = new Color(C_C.r, C_C.g, C_C.b, 0.4f);
            var S_C = StrikeZonePanel.color;
            StrikeZonePanel.color = new Color(S_C.r, S_C.g, S_C.b, 0.5f);
            var P_C = PitcherZonePanel.color;
            PitcherZonePanel.color = new Color(P_C.r, P_C.g, P_C.b, 0.5f);
        }
        else
        {
            var C_C = CanvasPanel.color;
            CanvasPanel.color = new Color(C_C.r, C_C.g, C_C.b, 0f);
            var S_C = StrikeZonePanel.color;
            StrikeZonePanel.color = new Color(S_C.r, S_C.g, S_C.b, 0f);
            var P_C = PitcherZonePanel.color;
            PitcherZonePanel.color = new Color(P_C.r, P_C.g, P_C.b, 0f);
        }
    }

    public void ChangeText(bool isBatterTurn)
    {
        if (Player1isPitcher && isBatterTurn)
        {
            PitcherOrBatter.text = "プレイヤー2はタイミングよくバットを振ってください";
            IsBatterReadly.text = "プレイヤー2に端末を渡してください";
        }
        else if(!Player1isPitcher && isBatterTurn)
        {
            PitcherOrBatter.text = "プレイヤー1はタイミングよくバットを振ってください";
            IsBatterReadly.text = "プレイヤー1に端末を渡してください";
        }

        if(Player1isPitcher && !isBatterTurn)
        {
            PitcherOrBatter.text = "プレイヤー1はボールの軌道を描いてください";
            IsBatterReadly.text = "プレイヤー2に端末を渡してください";
        }
        else if(!Player1isPitcher && !isBatterTurn)
        {
            PitcherOrBatter.text = "プレイヤー2はボールの軌道を描いてください";
            IsBatterReadly.text = "プレイヤー1に端末を渡してください";
        }
    }

    void IningShow()
    {
        if(IningCount % 2 == 1)
        {
            Ining.text = ((IningCount / 2) + 1) + "回裏";
        }
        else
        {
            Ining.text = ((IningCount / 2) + 1) + "回表";
        }
    }

    void GameSet()
    {
        GoogleAdsScript.ShowInterstitialAd();
        if (PlayerPoints.x > PlayerPoints.y)
        {
            Winner.text = "プレイヤー１の勝利";
        }
        else if (PlayerPoints.x < PlayerPoints.y)
        {
            Winner.text = "プレイヤー１の勝利";
        }
        else if(PlayerPoints.x == PlayerPoints.y)
        {
            Winner.text = "ひきわけ";
        }
        GameSetPanel.anchoredPosition = Vector2.zero;
        audioSource.PlayOneShot(GameSetSound);
    }

    public void GotoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
