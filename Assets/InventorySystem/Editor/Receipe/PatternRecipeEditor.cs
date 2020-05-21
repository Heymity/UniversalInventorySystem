using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatternRecipe))]
public class PatternRecipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            PatternRecipeEditorWindow.Open((PatternRecipe)target);
        }
    }
}
