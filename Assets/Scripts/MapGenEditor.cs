using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate Map"))
        {
            (target as MapGenerator).GenerateMap();
        }
        if (GUILayout.Button("Build Physical Map"))
        {
            (target as MapGenerator).BuildPhysicalMap();
        }
        if (GUILayout.Button("Clear Physical Map"))
        {
            (target as MapGenerator).ClearPhysicalMap();
        }
    }
}
