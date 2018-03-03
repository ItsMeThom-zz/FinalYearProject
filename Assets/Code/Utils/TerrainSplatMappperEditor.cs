using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code.Utils
{
    using UnityEditor;
    using UnityEngine;


    [CustomEditor(typeof(TerrainSplatMapper))]
    public class TerrainSplatMapperEditor : Editor
    {
        private GameObject obj;
        private TerrainSplatMapper objScript;

        void OnEnable()
        {
            obj = Selection.activeGameObject;
            objScript = obj.GetComponent<TerrainSplatMapper>();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            //
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("P : Paint Terrain", GUILayout.MinWidth(80), GUILayout.MaxWidth(350)))
            {
                objScript.PaintTerrain();
            }

            EditorGUILayout.EndHorizontal();
        }


        void OnSceneGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.P:
                        objScript.PaintTerrain();
                        break;

                    default:

                        break;
                }
            }
        }
    }

}
