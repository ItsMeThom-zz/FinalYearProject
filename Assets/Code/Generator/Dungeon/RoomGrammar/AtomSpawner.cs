using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// On Awake this object spawns the specified atom.
/// AtomSpawner is used to replace nested atom prefabs with their base
/// as child prefabs lose connection to their original when nested.
/// </summary>
/// 

namespace GenerativeGrammar
{
    public class AtomSpawner : MonoBehaviour
    {
        public bool DrawLayout = true;
        public float Width = 1f;
        public float Height = 1f;
        public float Depth = 0.001f;
        public Color LinesColor = Color.red;

        public GameObject Spawn;

        private void Awake()
        {
            var newAtom = Instantiate(Spawn);
            newAtom.transform.SetParent(this.transform.parent, false);
            newAtom.transform.localPosition = this.transform.localPosition;
            Destroy(this.gameObject);
        }

        private void OnDrawGizmos()
        {

            if (DrawLayout)
            {
                Gizmos.color = LinesColor;
                Gizmos.DrawCube(this.transform.position, new Vector3(Width, Depth, Height));
            }

        }


    }
}