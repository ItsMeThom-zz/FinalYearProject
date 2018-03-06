// Create a semi transparent rectangle that lets you modify
// the "range" var that resides in "SolidRectangleExample.js"
using UnityEditor;
using UnityEngine;


class ExitMarker : MonoBehaviour
{

    public bool IsVisible = false;
    Vector3 DoorSize = new Vector3(2f, 2.5f, 0f);

    void OnDrawGizmosSelected()
    {
        if (IsVisible)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, DoorSize);
            //Gizmos.DrawWireCube(transform.position, transform.forward + new Vector3(2f, 2.5f));
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1 + Vector3.zero);
        }
    }
}