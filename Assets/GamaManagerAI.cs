using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GamaManagerAI : MonoBehaviour
{
    public GameObject Ads;
    GoogleAds GoogleAdsScript;

    LineRenderer lineRenderer;

    public LineRenderer[] ThrowLines = new LineRenderer[11];

    public RectTransform GoSign;

    private Vector2 StartPos_Of_Image = new Vector2(-1320f, 0);
    private Vector2 EndPos_Of_Image = new Vector2(0, 0);

    private bool isShowGoSignImage = false;
    private bool oneshot = false;

    public RectTransform SwingButton;
    public RectTransform GameSetPanel;

    public Text CountBall;
    public Text GetPointText;

    AudioSource audioSource;
    public AudioClip Syoubu_Sound;
    public AudioClip StepSound;
    public AudioClip StartSound;
    public AudioClip GameSetSound;

    public GameObject Pitcher;
    PitcherScript pitcherScript;

    public GameObject Batter;
    BatterScript batterScript;

    float delayTime = 0.5f;

    [HideInInspector]
    public int GetPoint = 0;
    int BallCount = 0;

    bool IsnormalPosition = false;

    void Start()
    {
        pitcherScript = Pitcher.GetComponent<PitcherScript>();
        batterScript = Batter.GetComponent<BatterScript>();
        audioSource = GetComponent<AudioSource>();
        GoogleAdsScript = Ads.GetComponent<GoogleAds>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsnormalPosition)
        {
            Invoke("CreandAndThrow", 2f);
            IsnormalPosition = false;
        }

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

    void CreandAndThrow()
    {
        if(BallCount == 6)
        {
            return;
        }
        WhenLineMade();
        ShowGoSignImage();
    }

    public void ChangeBool_IsNormalPosition()
    {
        IsnormalPosition = true;
    }

    public void WhenLineMade()
    {
        int number = Random.Range(0, 13);
        var ThrowLine = Instantiate(ThrowLines[number]);
        lineRenderer = ThrowLine;
        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
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
    }

    public void SetPitcherNormalPosition()
    {
        IsnormalPosition = true;
        BallCount++;
        pitcherScript.animator.SetTrigger("Back");
        batterScript.animator.SetTrigger("Back");
        batterScript.Simulation_SwingTime = 0;
        GameObject touchField = GameObject.FindWithTag("Canvas");
        GameObject Line = GameObject.FindWithTag("SendLine");
        Destroy(Line);
        if (BallCount == 6)
        {
            GameSet();
            IsnormalPosition = false;
        }
        CountBall.text = (BallCount + 1) + "球目";
    }

    void GameSet()
    {
        GoogleAdsScript.ShowInterstitialAd();
        GetPointText.text = "得点：" + GetPoint;
        GameSetPanel.anchoredPosition = Vector2.zero;
        audioSource.PlayOneShot(GameSetSound);
    }

    public void ShowGoSignImage()
    {
        if (!isShowGoSignImage)
        {
            isShowGoSignImage = true;
            audioSource.PlayOneShot(Syoubu_Sound);
        }
        else
        {
            isShowGoSignImage = false;
        }

    }

    public void PanelAppear(RectTransform rect)
    {
        rect.anchoredPosition = Vector2.zero;
    }

    public void PanelDelete(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0, 1600);
    }

    public void GotoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

}
