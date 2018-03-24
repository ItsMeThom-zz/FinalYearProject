using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class EditorList
    {
        public static void Show(SerializedProperty list)
        {
            
            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty rule = list.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(rule);
                EditorGUI.indentLevel += 1;
                SerializedProperty ruleWeight = rule.FindPropertyRelative("Weight");
                SerializedProperty ruleObject = rule.FindPropertyRelative("Type");
                //SerializedProperty ruleRewrite = rule.FindPropertyRelative("Rewrite");
                EditorGUILayout.PropertyField(ruleWeight);
                EditorGUILayout.PropertyField(ruleObject);
                //EditorGUILayout.PropertyField(ruleRewrite);
                EditorGUI.indentLevel -= 1;
                //for(int j = 0; j <)
                //var rule = new SerializedObject(list.GetArrayElementAtIndex(i));

            }
            EditorGUI.indentLevel -= 1;
        }
    }
}
