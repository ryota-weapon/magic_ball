using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoneScript : MonoBehaviour
{
    [HideInInspector]
    public bool Pressed;
    public Text TestText;
    void Start()
    {
        
    }

    void Update()
    {

    }


    public bool PressedCheck() //マウスのポジションが自分の上にあるかを調べる
    {
        /* int layerMask = LayerMask.GetMask(new string[] { "UI" });
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         RaycastHit[] hits = Physics.RaycastAll(ray,layerMask);
         foreach(RaycastHit hit in hits)
         {
             print(this.gameObject.name + " touched");
             if (hit.collider == this.gameObject)
             {
                 print(this.gameObject.name + " touched");
                 return true;
             }
         }
  
         */
        return false;


    }
}
