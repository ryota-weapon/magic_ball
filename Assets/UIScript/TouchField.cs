
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [HideInInspector]
    public bool Pressed;

    public Text TestText;

    public bool IsBake = false;

    public GameObject PitcherZone;
    public GameObject StrikeZone;
    public GameObject Line;
    public GameObject Ball;
    GameObject LineObject;

    ZoneScript PitcherZone_script;
    ZoneScript StrikeZone_script;

    void Start()
    {

        PitcherZone_script = PitcherZone.GetComponent<ZoneScript>();
        StrikeZone_script = StrikeZone.GetComponent<ZoneScript>();

    }


    void Update()
    {
        if (Pressed)
        {

            if (!TouchCheck())
            {
                Pressed = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsBake)
        {
            return;
        }

        //UIに向かってRayを飛ばす
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, result);

        foreach (RaycastResult raycastResult in result)
        {
            if (raycastResult.gameObject.name == PitcherZone.name)
            {
                Pressed = true;
                TestText.text = "Now Touched";
                LineObject = Instantiate(Line, Vector3.zero, Quaternion.identity);
                LineRenderScript lineRenderScript = LineObject.GetComponent<LineRenderScript>();
                lineRenderScript.DrawLine();
            }
        }



    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
        TestText.text = "Not Touched";
        if (!IsBake)
        {
            DeleteLine();
        }
    }

    private bool TouchCheck() //曲線を書いてて枠からはみ出たかorストライクゾーンまで書いたか検出
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, result);

        foreach (RaycastResult raycastResult in result)
        {

            if(raycastResult.gameObject.name == StrikeZone.name)
            {
                TestText.text = "Strike!";
                LineRenderScript lineRenderScript = LineObject.GetComponent<LineRenderScript>();
                lineRenderScript.LineFixing();
                IsBake = true;
                return false;
            }
            else if (raycastResult.gameObject.name == this.gameObject.name)
            {
 
                return true;
            }
        }
        TestText.text = "Not Touched";
        DeleteLine();
        return false;
    }

    public void DeleteLine()
    {
        GameObject LineOb = GameObject.FindWithTag("SendLine");
        Destroy(LineOb);
        IsBake = false;
    }

    public void StartThrow()
    {
        Instantiate(Ball);
    }

}
