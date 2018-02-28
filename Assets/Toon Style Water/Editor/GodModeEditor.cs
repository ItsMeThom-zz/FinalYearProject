using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GodMode))]
public class GodModeEditor: Editor
{

    public override void OnInspectorGUI()  {
        ef.start();
        GodMode tmp = (GodMode)target;

            ef.write("speed:"); ef.edit(ref tmp.speed); ef.newLine();
            ef.write("sensitivity:"); ef.edit(ref tmp.sensitivity); ef.newLine();

            ef.write("WASD - move"); ef.newLine();
            ef.write("Shift, Space - Dwn, Up"); ef.newLine();
            ef.write("RMB - look around"); ef.newLine();
           
            ef.newLine();

  
    }
}
