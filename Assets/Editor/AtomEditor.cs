using Assets.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GenerativeGrammar
{
    [CustomEditor(typeof(Atom))]
    [CanEditMultipleObjects]
    public class AtomEditor : Editor
    {


        Atom atom;
        SerializedObject GetTarget;
        SerializedProperty RuleList;
        int ListSize;

        void OnEnable()
        {
            atom = (Atom)target;
            GetTarget = new SerializedObject(atom);
            //RuleList = GetTarget.FindProperty("RuleSet");
            //Debug.Log("RS: " + RuleList.arraySize);
            // Find the List in our script and create a refrence of it
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            //Update our list
            GetTarget.Update();
            EditorGUILayout.HelpBox("UI Layout Drawing", MessageType.None);
            EditorGUILayout.PropertyField(GetTarget.FindProperty("DrawLayout"));
            EditorGUILayout.PropertyField(GetTarget.FindProperty("Width"));
            EditorGUILayout.PropertyField(GetTarget.FindProperty("Height"));
            EditorGUILayout.PropertyField(GetTarget.FindProperty("Depth"));
            EditorGUILayout.PropertyField(GetTarget.FindProperty("LinesColor"));

            EditorGUILayout.HelpBox("Atom Rules", MessageType.None);
            EditorList.Show(GetTarget.FindProperty("RuleSet"));

            //Apply the changes to our list
            GetTarget.ApplyModifiedProperties();
        }


    }
}
