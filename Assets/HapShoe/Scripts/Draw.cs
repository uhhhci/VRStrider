using UnityEngine;
using System.Collections;

/// Modified version of: https://gist.github.com/nsdevaraj/5877269

public class Draw : MonoBehaviour
{
    struct GUILine
    {
        public Vector2 startPt;
        public Vector2 endPt;
    }

    private GUILine newline;
    private ArrayList lines;
    private float length;
    private float angle;
    private Texture2D lineTex;
    private Matrix4x4 matrixBackup;
    public float width = 2.0f;

    void Start()
    {
        lines = new ArrayList();
        lineTex = new Texture2D(1, 1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (newline.startPt == Vector2.zero)
            {
                newline.startPt = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            else
            {
                newline.endPt = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (newline.endPt != Vector2.zero)
            {
                //Debug.Log(newline.startPt.ToString() + " " + newline.endPt.ToString());
                lines.Add(newline);
                newline = new GUILine();
            }
        }
    }

    void OnGUI()
    {
        foreach (GUILine line in lines)
        {
            DrawLine (line.startPt, line.endPt);
        }
    }
    
    void DrawLine(Vector2 pointA, Vector2 pointB)
    {
        pointA.y = Screen.height - pointA.y;
        pointB.y = Screen.height - pointB.y;
        angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * 180f / Mathf.PI;
        length = (pointA - pointB).magnitude;
        GUI.color = Color.green;
        matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), lineTex);
        GUI.matrix = matrixBackup;
    }
}