using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitcherScript : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    public GameObject Ball;

    public GameObject StrikeZone;
    StrikeZoneCollider StrikeZone_Script;

    AudioSource audioSource;
    public AudioClip Throw;

    private bool GenelateBall = false;

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        StrikeZone_Script = StrikeZone.GetComponent<StrikeZoneCollider>();
    }


    void Update()
    {
        if (GenelateBall && animator.GetCurrentAnimatorStateInfo(0).IsName("BallGene"))
        {
            GameObject ball = Instantiate(Ball);
            StrikeZone_Script.Ball = ball;
            GenelateBall = false;
            audioSource.PlayOneShot(Throw);

        }
    }

    public void StartThrow()
    {
        GenelateBall = true;
        animator.SetTrigger("Throw");
    }
}
