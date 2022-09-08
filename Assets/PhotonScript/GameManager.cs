using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;
// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject Ads;
    GoogleAds GoogleAdsScript;
    /// <summary>
    /// シミュレーションを行うためのパラメータたち、通信で使う
    ///
    /// </summary>
    [HideInInspector]
    public Vector3 PowerVector;
    [HideInInspector]
    public float SwingTime = 0;
    [HideInInspector]
    public bool Other_Player_isHit = false;
    [HideInInspector]
    public bool Other_Player_isJustMeet = false;
    [HideInInspector]
    public Vector3 Ball_Position;
    [HideInInspector]
    public bool NextIningIsLast = false;

    [HideInInspector]
    public Vector2 PlayerPoints = Vector2.zero;
    [HideInInspector]
    public int[] PlayersRate = new int[2];

    private bool Ready_to_Throw = false;　//ピッチャーが投げ始めるサイン

    [HideInInspector]
    public bool ChangeCheck;
    private bool OtherChengeCheck;

    //準備OK！という画像を出すため 検知用bool値
    private bool isShowGoSignImage = false;
    private bool oneshot = false;

    /// <summary>
    /// ローカルでシミュレーションするスタートサイン
    /// ↑（ピッチャー側でバッターがどのような処置を取ったか再生する感じ）
    /// </summary>
    /// 
    [HideInInspector]
    public  bool Ready_To_StartSimulation = false;

    public Text CountBall;
    public Text MyPoint;
    public Text OthersPoint;
    public Text Winner;
    public Text Ining;
    public Text[] Rates = new Text[2];
    public Text LeftTime;
    public Text GetRate;
    public Text AllPlayerNum;
    public Text[] PlayerName = new Text[2];

    public RectTransform GameSetPanel;

    [HideInInspector]
    public bool isPitcher = true;

    public RectTransform Change;
    public RectTransform BallSendPanel;
    public RectTransform DisConnectPanel;
    public RectTransform ThisPlayerLeft;

    LineRenderer lineRenderer;
    public LineRenderer GetLineRenderer;

    public RectTransform GoSign;
    private Vector2 StartPos_Of_Image =  new Vector2(-1320f, 0);
    private Vector2 EndPos_Of_Image =new Vector2(0, 0);

    public RectTransform MachingPanel;
    public Text[] PlayerAndOtherPlayer = new Text[2];

    public GameObject Button_Go_to_Title;

    private int indexCount = 0;

    public GameObject Pitcher;
    PitcherScript pitcherScript;

    public GameObject Batter;
    BatterScript batterScript;

    public GameObject StrikeZone;
    StrikeZoneCollider strikeZoneScript;

    public GameObject BallFollowCamera;
    BallCamera ballCameraScript;

    public Image CanvasPanel;
    public Image StrikeZonePanel;
    public Image PitcherZonePanel;

    GameObject Ball;

    private bool GameIsOver = false; 

    int soundNum;

    public RectTransform SwingButton;

    public GameObject DefaultLine;

    float delayTime = 1.0f;

    private float t,t2 = 0;
    bool SetNormalPosition = false;

    [HideInInspector]
    public int count = 0;
    int IningCount = 0;


    bool oneshot1 = false;
    bool isStartGame = false;

    AudioSource audioSource;
    public AudioClip StepSound;
    public AudioClip Syoubu_Sound;
    public AudioClip Start_Sound;
    public AudioClip GameSetSound;

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        pitcherScript = Pitcher.GetComponent<PitcherScript>();
        batterScript = Batter.GetComponent<BatterScript>();
        strikeZoneScript = StrikeZone.GetComponent<StrikeZoneCollider>();
        ballCameraScript = BallFollowCamera.GetComponent<BallCamera>();
        PlayersRate[0] = PlayerPrefs.GetInt("PlayerRate");
        Rates[0].text = PlayersRate[0] + "";
        audioSource = GetComponent<AudioSource>();
        GoogleAdsScript = Ads.GetComponent<GoogleAds>();
    }
    
    void Update()
    {
        if (ChangeCheck && !isPitcher)
        {
            Invoke("delayMethod", 0.5f);
            ChangeCheck = false;
        }

        if(isPitcher && SetNormalPosition)
        {
            t2 += Time.deltaTime;
            float Left_Time = Mathf.Max(Mathf.CeilToInt(15 - t2), 0);
            LeftTime.text = "" + Left_Time;
            if(Left_Time == 0)
            {
                TouchField touchField = GameObject.FindWithTag("Canvas").GetComponent<TouchField>();
                touchField.DeleteLine();
                Instantiate(DefaultLine);
                When_LineCreate_Over();
                ChangeAlfa(false);
                SetPanelPosition(BallSendPanel);
            }
        }

        if (!isStartGame)
        {
            AllPlayerNum.text = "同時接続人数:" + PhotonNetwork.CountOfPlayers +"人";
        }
    }

    private void FixedUpdate()
    {
        if (isShowGoSignImage)
        {

            GoSign.anchoredPosition = Vector2.Lerp(GoSign.anchoredPosition, EndPos_Of_Image, 0.1f);
            if (EndPos_Of_Image.x - GoSign.anchoredPosition.x < 1.0f && !oneshot)
            {
                oneshot = true;

                if (!isPitcher)
                {
                    delayTime = 0.25f;
                }
                else
                {
                    delayTime = 0.5f;
                }

                Invoke("ShowGoSignImage", delayTime);
            }
        }
        else
        {
            GoSign.anchoredPosition = Vector2.Lerp(GoSign.anchoredPosition, StartPos_Of_Image, 0.1f);
            if (GoSign.anchoredPosition.x - StartPos_Of_Image.x < 1.0f && oneshot)
            {
                oneshot = false;
                if (!isPitcher)
                {
                    Invoke("BeReadyToThrow", delayTime);
                }
            }
        }

        if (Ready_To_StartSimulation && isPitcher)
        {

            if (t == 0)
            {
       
                BeReadyToThrow();
            }
            t += Time.fixedDeltaTime;
  

            if (t >= SwingTime && isPitcher)
            {
                batterScript.animator.SetTrigger("Swing");

                if (Other_Player_isJustMeet)
                {
                    soundNum = 0;
                }
                else if (Other_Player_isHit)
                {
                    soundNum = 1;
                }
                else
                {
                    soundNum = 2;
                    batterScript.animator.SetTrigger("NonHit");
                }
                Invoke("SoundGene", 0.35f);

                t = 0;
                Ready_To_StartSimulation = false;

            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsConnected && !GameIsOver)
        {
            float Dis = RateChange(true);
            NCMBRateChange(PlayerPrefs.GetInt("PlayerRate"));
            GetRate.text = "獲得レート" + Mathf.CeilToInt(Dis);
            DisConnectPanel.anchoredPosition = Vector2.zero;
        }
    }

    public override void OnLeftRoom()
    {
        if (!GameIsOver)
        {
            ThisPlayerLeft.anchoredPosition = Vector2.zero;
        }
    }
    private void delayMethod()
    {
        photonView.RPC(nameof(ChangePutton), RpcTarget.All);

    }
    [PunRPC]
    private void ChangePutton()
    {

        Change.anchoredPosition = new Vector2(1600, 0);
        audioSource.PlayOneShot(Start_Sound);
    }
    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(null, roomOptions);
        print("I made a room!");
    }
    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            isPitcher = true;
            ChangeAlfa(true);
        }
        else
        {
            print(PhotonNetwork.PlayerList.Length);
            isPitcher = false;
            ChangeAlfa(false);
        }
        PlayerAndOtherPlayer[0].text = PlayerPrefs.GetString("PlayerName");
        PlayerName[0].text = PlayerPrefs.GetString("PlayerName");

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            photonView.RPC(nameof(GetAwayMatchingPanel), RpcTarget.Others, PlayerAndOtherPlayer[0].text, PlayerPrefs.GetInt("PlayerRate"),false);
        }
    }
    void ChangePictherAndBatter(bool ispitcher)
    {
        if (isPitcher)
        {
            isPitcher = false;
        }
        else
        {
            isPitcher = true;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {


        }
    }

    public void When_LineCreate_Over()
    {
        SetNormalPosition = false;
        t2 = 0;
        lineRenderer = GameObject.FindWithTag("SendLine").GetComponent<LineRenderer>();
        photonView.RPC(nameof(GotLine_Initialization), RpcTarget.All);
        for (int i = 0;i < lineRenderer.positionCount; i++)
        {
            photonView.RPC(nameof(GotLine_Photon), RpcTarget.Others, i, lineRenderer.GetPosition(i));
        }

        photonView.RPC(nameof(DoShowGoSignImage), RpcTarget.All);

        indexCount = 0;
    }

    [PunRPC]
    void GetAwayMatchingPanel(string otherPlayersName, int PlayerRate,bool Switch)
    {
        if (otherPlayersName != PlayerAndOtherPlayer[0].text)
        {
            PlayerAndOtherPlayer[1].text = otherPlayersName;
            PlayerName[1].text = otherPlayersName;
            PlayersRate[1] = PlayerRate;
            Rates[1].text = "" + PlayersRate[1];
            Invoke("JustDelay", 1.0f);
            Button_Go_to_Title.SetActive(false);
        }
        float NewRate = RateChange(false);
        NCMBRateChange(PlayerPrefs.GetInt("PlayerRate"));
        isStartGame = true;
        if (Switch)
        {
            return;
        }
  

        photonView.RPC(nameof(GetAwayMatchingPanel), RpcTarget.Others, PlayerAndOtherPlayer[0].text, PlayerPrefs.GetInt("PlayerRate"), true);

    }

    void JustDelay()
    {
        SetPanelPosition(MachingPanel);
        SetNormalPosition = true;
    }

    [PunRPC]
    private void GotLine_Photon(int Index,Vector3 position)
    {
        indexCount++;
        GetLineRenderer.positionCount = indexCount;
        GetLineRenderer.SetPosition(Index, position);
    }

    [PunRPC]
    private void GotLine_Initialization()
    {
        indexCount = 0;
        GetLineRenderer.positionCount = 1;
    }

    [PunRPC]
    private void DoShowGoSignImage()
    {
        float delayTime = 0f;
        if (isPitcher)
        {
            delayTime = 2.5f;
        }

        Invoke("ShowGoSignImage", delayTime);
    }



    private void ShowGoSignImage()
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

    private void BeReadyToThrow()
    {
        if (!isPitcher || Ready_To_StartSimulation)
        {
            Ready_to_Throw = true;
            pitcherScript.StartThrow();
            isShowGoSignImage = false;
            batterScript.animator.SetTrigger("Ready");
            batterScript.StartSimulation = true;
            batterScript.Simulation_SwingTime = 0;
            audioSource.PlayOneShot(StepSound);
            ChangeAlfa(false);
            if (IningCount == 3)
            {
                NextIningIsLast = true;
            }
            if (!isPitcher)
            {
                SwingButton.anchoredPosition = new Vector2(0, -260f);
            }
        }
        else
        {

        }
    }

    public void SetPanelPosition(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0, -750f);
    }
    public void SetPitcherNormalPosition()
    {
        if (GameIsOver)
        {
            return;
        }
        count++;
        SetNormalPosition = true;
        LeftTime.text = "15";
        pitcherScript.animator.SetTrigger("Back");
        batterScript.animator.SetTrigger("Back");
        batterScript.Simulation_SwingTime = 0;
        GameObject touchField = GameObject.FindWithTag("Canvas");
        TouchField touchFieldScript = touchField.GetComponent<TouchField>();
        oneshot1 = false;
        touchFieldScript.DeleteLine();
        if (count == 3)
        {
            IningCount++;
            if (IningCount >= 4)
            {
                GameSet();
                return;
            }
            ChangePictherAndBatter(true);
            IningShow();
            count = 0;
        }
        if (isPitcher)
        {
            ChangeAlfa(true);
            ChangeCheck = true;
        }
        CountBall.text = (count + 1) + "球目";

    }

    [PunRPC]
    public void StartBallSimulation(Vector3 Power, float SimulationTime, bool isHit, bool isJustMeet, Vector3 BallPosition)
    {
        if (Pitcher)
        {
            PowerVector = Power;
            SwingTime = SimulationTime;
            Other_Player_isHit = isHit;
            Other_Player_isJustMeet = isJustMeet;
            Ball_Position = BallPosition;
            Ready_To_StartSimulation = true;
        }
    }


    private void SoundGene() 
    {

        strikeZoneScript.Sound(soundNum);
        if (Other_Player_isHit)
        {
            Ball = GameObject.FindWithTag("Ball");
            BallScript ballScript = Ball.GetComponent<BallScript>();
            Ball.transform.position = Ball_Position;
            ballScript.BallShot(PowerVector);
            BallFollowCamera.SetActive(true);
            ballCameraScript.FindBallAndFollowIt();
        }
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
            PitcherZonePanel.color = new Color(P_C.r, P_C.g, P_C.b,0f);
        }
    }

    void GameSet()
    {
        GoogleAdsScript.ShowInterstitialAd();
        GameIsOver = true;
        if (PlayerPoints.x > PlayerPoints.y)
        {
            Winner.text = "勝利！";
            float Dis = RateChange(true);
            Rates[0].text = PlayersRate[0] + "→" + PlayerPrefs.GetInt("PlayerRate");
            Rates[1].text = PlayersRate[1] + "→" + (PlayersRate[1] - Dis);
            NCMBRateChange(PlayerPrefs.GetInt("PlayerRate"));
        }
        else if (PlayerPoints.x < PlayerPoints.y)
        {
            Winner.text ="敗北";
            float Dis = RateChange(false);
            Rates[0].text = PlayersRate[0] + "→" + PlayerPrefs.GetInt("PlayerRate");
            Rates[1].text = PlayersRate[1] + "→" + (PlayersRate[1] + Dis);
            NCMBRateChange(PlayerPrefs.GetInt("PlayerRate"));
        }
        else if (PlayerPoints.x == PlayerPoints.y)
        {
            Winner.text = "ひきわけ";
            NCMBRateChange(PlayerPrefs.GetInt("PlayerRate"));
        }

        GameSetPanel.anchoredPosition = Vector2.zero;
        audioSource.PlayOneShot(GameSetSound);
    }

    public void GotoTitle()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("TitleScene");
        
    }

    void IningShow()
    {
        if (IningCount % 2 == 1)
        {
            Ining.text = ((IningCount / 2) + 1) + "回裏";
        }
        else
        {
            Ining.text = ((IningCount / 2) + 1) + "回表";
        }
    }

    float RateChange(bool isWin)
    {
        float GotPoint;
        if (isWin)
        {
            GotPoint = Mathf.Abs(RateCal());
            int Mypoint = PlayersRate[0] + Mathf.CeilToInt(GotPoint);
            PlayerPrefs.SetInt("PlayerRate",Mypoint); 
        }
        else
        {
            GotPoint = Mathf.Abs(RateCal());
            int Mypoint = PlayersRate[0] - Mathf.CeilToInt(GotPoint);
            PlayerPrefs.SetInt("PlayerRate", Mypoint);
        }
        PlayerPrefs.Save();
        return GotPoint;
    }

    float RateCal()
    {
        float bias = (PlayersRate[1] - PlayersRate[0]) / 10f;
        float Defo = 10;
        float gotPoint = Defo + bias;
        return gotPoint;
    }

    void NCMBRateChange(int NewRate)
    {
        NCMBObject MyRate = new NCMBObject("PlayerRate");
        MyRate.ObjectId = PlayerPrefs.GetString("PlayerID");
        MyRate.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                //成功時の処理
                MyRate["Rate"] = NewRate;
                MyRate.SaveAsync();
            }
        });
    }
}