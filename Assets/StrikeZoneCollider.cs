using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StrikeZoneCollider : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public bool Ball_Enter = false;
    [HideInInspector]
    public GameObject Ball;

    AudioSource audioSource;
    public AudioClip Bat_HitSound;
    public AudioClip Bat_Not_Hit_Sound;
    public AudioClip Bat_JustMeet;

    public GameObject BallFollowCamera;
    BallCamera ballCameraScript;

    int BallDistance = 0;

    [SerializeField]
    bool NotOnline = false;
    [SerializeField]
    bool IsAIGame = false;

    [HideInInspector]
    public Vector3 PowerVector = Vector3.zero;

    public GameObject gameManager;
    GameManager gameManager_script;

    private float power = 1100f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball")
        {
            Ball_Enter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Ball_Enter = false;
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ballCameraScript = BallFollowCamera.GetComponent<BallCamera>();
        gameManager_script = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public bool IfBatterSwing(float SwingTime)
    {
        if(Ball != null && Ball_Enter)
        {
            float distance = Ball.transform.position.x - this.transform.position.x;
            // float distance = Ball.transform.position.z - this.transform.position.z;
            print("distance = " + distance);
            // print("はやい" if distance >= 0 else "おそい");

            if (Mathf.Abs(distance) < 0.1f) //ボールの強さ計算
            {
                audioSource.PlayOneShot(Bat_JustMeet);
                PowerVector = MakeVector3(distance, true); //ジャストミート
                if (!NotOnline)//オンライン
                {
                    BallScript ballScript = Ball.GetComponent<BallScript>();
                    ballScript.BallShot(PowerVector);
                }
                else
                {
                    if (!IsAIGame)//2人対戦
                    {
                        BallScript_NotOnline ballScript2 = Ball.GetComponent<BallScript_NotOnline>();
                        ballScript2.BallShot(PowerVector);
                    }
                    else//練習用　AI対戦
                    {
                        BallScriptAI ballScriptAI = Ball.GetComponent<BallScriptAI>();
                        ballScriptAI.BallShot(PowerVector);
                    }
                }
                BallFollowCamera.SetActive(true);
                ballCameraScript.FindBallAndFollowIt();
            //    print("Swingtime = " + SwingTime);
                if (!NotOnline)
                {
                    gameManager_script.photonView.RPC(nameof(gameManager_script.StartBallSimulation), RpcTarget.Others, PowerVector, SwingTime, true, true, Ball.transform.position);
                }
                print(Ball.transform.position);
                return true;
            }
            PowerVector = MakeVector3(distance, false);
            audioSource.PlayOneShot(Bat_HitSound);
            if (!NotOnline)
            {
                BallScript ballScript = Ball.GetComponent<BallScript>();
                ballScript.BallShot(PowerVector);
            }
            else
            {
                if (!IsAIGame)
                {
                    BallScript_NotOnline ballScript2 = Ball.GetComponent<BallScript_NotOnline>();
                    ballScript2.BallShot(PowerVector);
                }
                else
                {
                    BallScriptAI ballScriptAI = Ball.GetComponent<BallScriptAI>();
                    ballScriptAI.BallShot(PowerVector);
                }
            }
            BallFollowCamera.SetActive(true);
         //   print("Swingtime = " + SwingTime);
            ballCameraScript.FindBallAndFollowIt();
            if (!NotOnline)
            {
                gameManager_script.photonView.RPC(nameof(gameManager_script.StartBallSimulation), RpcTarget.Others, PowerVector, SwingTime, true, false, Ball.transform.position);
            }
            print(Ball.transform.position);
            return true;
        }
        else
        {
            audioSource.PlayOneShot(Bat_Not_Hit_Sound);
         //   print("Swingtime = " + SwingTime);
            if (!NotOnline)
            {
                gameManager_script.photonView.RPC(nameof(gameManager_script.StartBallSimulation), RpcTarget.Others, Vector3.zero, SwingTime, false, false, Ball.transform.position);
            }
            return false;
        }
    }

    private Vector3 MakeVector3(float distance,bool isJustMeet)
    {
        if (isJustMeet)
        {
            float Power = power;
            Vector3 Dir = new Vector3(Random.Range(-0.9f,-1.0f), Random.Range(0.7f,1.0f), -distance) * Power;
            print(Dir);
            return Dir;
        }
        else
        {
            float Power = power * (1- Mathf.Abs(distance)/3);
            Vector3 Dir = new Vector3(Random.Range(-0.6f, -0.7f) * Mathf.Abs(1 - distance), Random.Range(0.5f, 0.6f)* Mathf.Abs(1 - distance), -distance/2)* Power;
            print(Dir);
            return Dir;
        }
    }

    public void Sound(int a)
    {
        switch (a)
        {
            case 0:
                audioSource.PlayOneShot(Bat_JustMeet);
                break;
            case 1:
                audioSource.PlayOneShot(Bat_HitSound);
                break;
            case 2:
                audioSource.PlayOneShot(Bat_Not_Hit_Sound);
                break;

        }
    }


}
