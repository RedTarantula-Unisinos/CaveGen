using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerScript))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate PetriNet"))
        {
            (target as PlayerScript).CreatePetriNet();
        }
        if (GUILayout.Button("Test PetriNet"))
        {
            (target as PlayerScript).TestRun();
        }

    }
}
        