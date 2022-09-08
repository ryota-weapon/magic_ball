using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LineRenderScript : MonoBehaviour
{

    private float deltaTime = 0f;
    private float touchInterval = 0.025f;


    private int lineIndex = 0;

    public LineRenderer lineRenderer;

    RectTransform Panel;
    GameObject InkGage;

    private Vector3 OldPos;
    private Vector3 NowPos;
    private float distance;
    private float AllDistance;
    
    private float Minimum_distance = 0.25f;

    Transform StrikeZonePos;

    GameObject LineCanvas; //曲線を書くキャンバスオブジェクト
    TouchField touchField;

    private float Ink = 0;
    public float MaxInk = 30f;

    private float DrawTime = 0;
    public float MaxTime = 10f;

    bool oneshot = false;

    void Start()
    {
        LineCanvas = GameObject.FindWithTag("Canvas");
        StrikeZonePos = GameObject.Find("StrikeZonePos").GetComponent<Transform>();
        touchField = LineCanvas.GetComponent<TouchField>();
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        Panel = GameObject.FindWithTag("InkGage").GetComponent<RectTransform>();
        InkGage = GameObject.Find("Ink");

    }


    void Update()
    {
        if (touchField.Pressed)
        {
            if (!oneshot)
            {
                Panel.anchoredPosition = Vector2.zero;
                oneshot = true;
            }
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1 = new RaycastHit();
            if (Physics.Raycast(ray1, out hit1))
            {
                if (hit1.collider.tag == "Wall")
                {
                    NowPos = hit1.point;
                }
            }
            distance = (OldPos - NowPos).magnitude;
            AllDistance += distance;
            DrawTime += Time.deltaTime;

            if (DrawTime > MaxTime)
            {
                touchField.DeleteLine();
                return;
            }
            if (distance >= Minimum_distance)
            {
                lineRenderer.positionCount = lineIndex + 1;
                lineRenderer.SetPosition(lineIndex, NowPos);
                Used_Ink_Calculate(lineIndex);
                lineIndex++;
                OldPos = NowPos;

            }
        }

     
       // print((Maxdistance - AllDistance) / Maxdistance);
    }

    private void FixedUpdate()
    {
      
    }

    public void DrawLine()
    {

        lineRenderer.enabled = true;
        lineRenderer.positionCount = lineIndex + 1;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Wall")
            {
                lineRenderer.SetPosition(lineIndex, hit.point);
                lineIndex++;
                OldPos = hit.point;
            }
        }
    }

    public void LineFixing() //ストライクゾーンとラインを補完する
    {
        Vector3 dy_dx_Goal = (StrikeZonePos.position - lineRenderer.GetPosition(lineIndex - 1)).normalized;
        Vector3 dir = (dy_dx_Goal).normalized * Minimum_distance; //大きさがMaxdistanceの方向ベクトル

        if ((lineRenderer.GetPosition(lineIndex - 1) - StrikeZonePos.position).magnitude > Minimum_distance)
        {
            lineRenderer.positionCount = lineIndex + 1;
            lineRenderer.SetPosition(lineIndex, lineRenderer.GetPosition(lineIndex - 1) + dir);
            lineIndex++;
            LineFixing();
        }
        else
        {
            lineRenderer.positionCount = lineIndex + 1;
            lineRenderer.SetPosition(lineIndex, StrikeZonePos.position);
            lineIndex++;
        }
        
        RectTransform SendButtonsPanel;
        SendButtonsPanel = GameObject.Find("SendButtonPanel").GetComponent<RectTransform>();
        SendButtonsPanel.anchoredPosition = new Vector2(0, 0);
        oneshot = false;
    }


    void Used_Ink_Calculate(int index)
    {
        if (index == 0) return;

        float Point_distance = (lineRenderer.GetPosition(index) - lineRenderer.GetPosition(index - 1)).magnitude;
        Ink += Point_distance;

        if (Ink > MaxInk)
        {
            touchField.DeleteLine();
        }

        InkGage.transform.localScale = new Vector3((MaxInk - Ink) / MaxInk, 1, 1);

    }
}
