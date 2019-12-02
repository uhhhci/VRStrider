
/**
 *  Haptics Framework
 *
 *  UHH HCI 
 *  Author: Oscar Ariza <ariza@informatik.uni-hamburg.de>
 *
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShoeManager))]
public class ShoeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShoeManager app = (ShoeManager)target;
 
        GUILayout.Space(15);

        if (GUILayout.Button("Trigger Audio Right Shoe"))
        {
            //app.SendCommandRight(2);
        }

        if (GUILayout.Button("Trigger Audio Left Shoe"))
        {
            //app.SendCommandLeft(2);
        }
    }
}
