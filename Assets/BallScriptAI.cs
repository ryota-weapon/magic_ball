using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BallScriptAI : MonoBehaviour
{
    LineRenderer Line;
    [HideInInspector]
    public int Index = 0;
    private int IndexSize;
    private Vector3 DirectionPos;
    private Vector3 OldPos;
    private Vector3 oldPos;
    private float need_time, time1 = 0;
    private float distance = 0.25f;

    private bool oneshot2 = false;
    private bool oneshot = false;

    AudioSource audioSource;
    public AudioClip MitSound;
    public AudioClip HitWall;
    public AudioClip Voice1;
    public AudioClip Voice2;

    private bool IsHit = false;
    private bool IsGetScore = false;

    bool HitGround = false;

    Rigidbody rigidbody;
    SphereCollider sphereCollider;

    GameObject gameManager;
    GamaManagerAI gameManager_Script;

    RectTransform rect;

    [HideInInspector]
    public float AllDistance;

    private float SPEED_COEFF = 1200f;
    private float speed = 15f;

    private bool ShotSound = false;
    private float t = 0;

    private void OnCollisionEnter(Collision collision)
    {

        switch (collision.gameObject.tag)
        {
            case "HitWall":

                GetScore(1);
                if (!ShotSound)
                {
                    audioSource.PlayOneShot(HitWall);
                }
                break;
            case "2PointWall":

                GetScore(2);
                if (!ShotSound)
                {
                    audioSource.PlayOneShot(HitWall);
                }
                break;
            case "3PointWall":

                GetScore(3);
                if (!ShotSound)
                {
                    audioSource.PlayOneShot(HitWall);
                }
                break;
            case "Ground":
                if (!ShotSound)
                {
                    audioSource.PlayOneShot(HitWall);
                }
                HitGround = true;
                break;
        }
        ShotSound = true;
    }
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gameManager_Script = gameManager.GetComponent<GamaManagerAI>();
        Line = GameObject.FindWithTag("SendLine").GetComponent<LineRenderer>();
        IndexSize = Line.positionCount; //indexSizeが１ならばindexは0のみ
        OldPos = Line.GetPosition(Index);
        this.transform.position = OldPos;
        Index++;
        DirectionPos = Line.GetPosition(Index);
        need_time = distance / speed;

        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        sphereCollider = this.gameObject.GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();

        //スピードを投球の軌道の距離に応じて更新する。
        print("Index" + IndexSize);
        print("speed" + speed);
        speed = SPEED_COEFF / IndexSize;
    }


    void FixedUpdate()
    {

        if (Index < IndexSize - 1 && !IsHit)
        {
            if (time1 > need_time)
            {
                Index++;
                OldPos = DirectionPos;
                DirectionPos = Line.GetPosition(Index);

                need_time = (DirectionPos - OldPos).magnitude / speed;
                time1 = 0;
            }
            else
            {
                oldPos = this.transform.position;
                time1 += Time.fixedDeltaTime;
                Vector3 dir = (DirectionPos - this.transform.position) * speed * Time.fixedDeltaTime;
                this.transform.position += dir;
            }
        }
        else if (Index == IndexSize - 1 && !IsHit)
        {
            if (!oneshot)
            {
                audioSource.PlayOneShot(MitSound);

                LineRenderer line = GameObject.FindWithTag("SendLine").GetComponent<LineRenderer>();
                RectTransform rect = GameObject.Find("SwingButton").GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, -500f);
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                Invoke("DestroyCommand", 2.5f);
                Invoke("ChangeAppear", 1.5f);
                oneshot = true;
            }

        }

        if (IsHit && rigidbody.velocity.magnitude < 1.0f && HitGround)
        {

            rigidbody.velocity *= 0.8f;
            if (rigidbody.velocity.magnitude < 0.1f && !oneshot2)
            {
                rigidbody.velocity = Vector3.zero;
                oneshot2 = true;
                Invoke("WhenBallStop", 0.5f);
            }
        }
        else if (IsHit && !IsGetScore)
        {
            if (this.transform.position.x < -38.4f)
            {
                GetScore(5);
            }
        }

        if (ShotSound)
        {
            t += Time.deltaTime;
            if (t > 0.1f)
            {
                t = 0;
                ShotSound = false;
            }
        }

    }

    public void BallShot(Vector3 Force)
    {
        IsHit = true;
        sphereCollider.isTrigger = false;
        rigidbody.useGravity = true;
        rigidbody.AddForce(Force);

    }

    private void DestroyCommand()
    {
        LineRenderer line = GameObject.FindWithTag("SendLine").GetComponent<LineRenderer>();
        line.startWidth = 0f;
        line.endWidth = 0f;
        gameManager_Script.SetPitcherNormalPosition();
        Destroy(this.gameObject);
    }

    private void WhenBallStop()
    {
        GetScore(1);
    }

    private void GetScore(int Point)
    {
        if (IsGetScore)
        {
            return;
        }
        IsGetScore = true;
        GameObject PointMan = GameObject.Find("Point");
        Text PointText = PointMan.GetComponent<Text>();
        rect = PointMan.GetComponent<RectTransform>();
        if (Point != 5)
        {
            PointText.text = Point + "ポイント！";
            audioSource.PlayOneShot(Voice1);
        }
        else
        {
            PointText.text = "ホームラン！\n" + Point + "ポイント！";
            audioSource.PlayOneShot(Voice2);
        }
        PointWrite(Point);
        rect.anchoredPosition = Vector2.zero;
        Invoke("DestroyCommand", 2.5f);
        Invoke("CameraSetActive", 1.5f);
    }

    private void CameraSetActive()
    {
        rect.anchoredPosition = new Vector2(0, 564f);
        GameObject Cam = GameObject.Find("BallCamera");
        Cam.transform.position = new Vector3(8, 3.02f, -4.82f);
        Cam.SetActive(false);
    }

    void PointWrite(int point)
    {
        gameManager_Script.GetPoint += point;
        Text ScoreText = GameObject.Find("PointText").GetComponent<Text>();
        ScoreText.text = "得点: " + gameManager_Script.GetPoint + "点";
    }
}