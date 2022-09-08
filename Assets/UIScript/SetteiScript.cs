using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetteiScript : MonoBehaviour
{
    public AudioSource audioSource;
    public Text SwingNum;
    public Slider VolumeSlider;
    public Slider SwingSlider;
    void Start()
    {
        if (PlayerPrefs.GetInt("ValueChange") == 1)
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume");
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            audioSource.volume = 0.1f;
        }

        if (PlayerPrefs.GetInt("ValueChange_2") == 1)
        {
            SwingSlider.value = PlayerPrefs.GetFloat("SwingSpeed");
        }
        else
        {
        }

    }


    void Update()
    {
        
    }

    public void ValueChanged_Sound()
    {
        PlayerPrefs.SetFloat("Volume", VolumeSlider.value);
        PlayerPrefs.SetInt("ValueChange", 1);
        audioSource.volume = PlayerPrefs.GetFloat("Volume");
        print(PlayerPrefs.GetFloat("Volume"));
    }

    public void ValueChanged_Swing()
    {
        PlayerPrefs.SetFloat("SwingSpeed", SwingSlider.value);
        PlayerPrefs.SetInt("ValueChange_2", 1);
        BatterScript.SwingDelayTime = PlayerPrefs.GetFloat("SwingSpeed");
        print(PlayerPrefs.GetFloat("SwingSpeed"));
        SwingNum.text = "" + SwingSlider.value;
    }

    public void PanelAway(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0, 2000);
    }

    public void PanelAppeared(RectTransform rect)
    {
        rect.anchoredPosition = new Vector2(0,0);
    }
}
