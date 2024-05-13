using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FowardCreation))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelScript myLevelScript = (LevelScript) target;

        EditorGUILayout.intField("");
        {
            
        }
    }
}
