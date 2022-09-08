
using UnityEngine;
using UnityEngine.UI;

public class BatterScript : MonoBehaviour
{
    public GameObject StrikeZone;
    StrikeZoneCollider StrikeZone_Script;

    public GameObject Game_Manager;
    GameManager GameManagerScript;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public float Simulation_SwingTime = 0;
    [HideInInspector]
    public bool StartSimulation;

    [HideInInspector]
    public bool isSwing = false;
    [HideInInspector]
    public static float SwingDelayTime = 0.35f;

    [SerializeField]
    bool NotOnline = false;
    void Start()
    {
        StrikeZone = GameObject.Find("StrikeZoneCollider");
        StrikeZone_Script = StrikeZone.GetComponent<StrikeZoneCollider>();
        animator = this.gameObject.GetComponent<Animator>();
        if (NotOnline)
        {
            return;
        }
        GameManagerScript = Game_Manager.GetComponent<GameManager>();
        if (PlayerPrefs.GetInt("ValueChange_2") == 1)
        {   
            SwingDelayTime = PlayerPrefs.GetFloat("SwingSpeed");
            print(PlayerPrefs.GetFloat("SwingSpeed"));
        }
        else
        {
            SwingDelayTime = 0.35f;
        }
    }


    void Update()
    {


      //  text.text = "FPS" + 1 / Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (NotOnline)
        {
            return;
        }
        if (StartSimulation && !GameManagerScript.isPitcher) //タイマー起動中 プレイヤーはバッター側
        {
            Simulation_SwingTime += Time.fixedDeltaTime;
        }
    }
    public void StartSwing()
    {
        StartSimulation = false;
        isSwing = true;
        animator.SetTrigger("Swing");
        Invoke("AccessToStrikeZoneScript", SwingDelayTime);

    }

    public void Strike_Not_Swing()
    {
        if (!isSwing)
        {
            StartSimulation = false;
            Invoke("AccessToStrikeZoneScript", SwingDelayTime);
        }

    }

    private void AccessToStrikeZoneScript()
    { 
        bool isHit = StrikeZone_Script.IfBatterSwing(Simulation_SwingTime);
        print("isHit " + isHit);
        if (isHit)
        {

        }
        else
        {
            animator.SetTrigger("NonHit");

        }
        isSwing = false;
    }
}
