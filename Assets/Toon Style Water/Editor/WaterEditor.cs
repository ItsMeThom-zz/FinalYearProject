using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(FoamyWater))]
public class WaterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        edf.start();
        FoamyWater trg = (FoamyWater)target;

        bool modified = false;

        edf.write("Thickness:", 70);
        modified |= edf.edit(ref trg._thickness, 5, 300);
        edf.newLine();
        edf.write("Noise:",50);
        modified |= edf.edit(ref trg._noise, 0, 2);
        edf.newLine();
        edf.write("Upscale:",50);
        modified |= edf.edit(ref trg._upscale, 1, 64);
        edf.newLine();
        edf.write("Shadow color:");
        modified |= edf.edit(ref trg.shadowColor);
        edf.newLine();
        edf.write("Shadow strength:", 100);
        modified |= edf.edit(ref trg.shadowStrength, 0.1f, 1);
        edf.newLine();
        if (modified) { trg.setFoamDynamics(); SceneView.RepaintAll(); }
        edf.newLine();
        edf.write("Water foam mask:");
        trg.foamTexture = (Texture2D)EditorGUILayout.ObjectField(trg.foamTexture, typeof(Texture2D), true);
        edf.newLine();
    }
}