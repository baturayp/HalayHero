using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SongInfo))]
public class TestScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SongInfo)target;

        GUILayout.Label("\nSet desired BPM above and recalculate\nDo it only once. Default is 60");
        if (GUILayout.Button("Recalculate BPM", GUILayout.Height(40)))
        {
            script.RecalculateBPM();
        }
        GUILayout.Label("Recalculate total number of beats\neverytime you change the script.");
        if (GUILayout.Button("Recalculate Total Hits", GUILayout.Height(40)))
        {
            script.RecalculateTotalHitCounts();
        }

    }
}