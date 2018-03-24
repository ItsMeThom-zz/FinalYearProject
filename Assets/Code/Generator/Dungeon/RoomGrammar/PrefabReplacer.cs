using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour {

    [SerializeField]
    public GameObject BasePrefab;
    private void Awake()
    {
        if(this.BasePrefab != null)
        {
            var old = this.gameObject.transform.Find(BasePrefab.name);
            if(old == null) { Debug.Log("This object doesnt have that prefab!"); }
            var newInstance = Instantiate(BasePrefab, old.parent);
            newInstance.transform.position = old.transform.position;
            newInstance.transform.rotation = old.transform.rotation;
            Destroy(old);
        }

    }
}
