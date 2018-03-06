using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlaneMesh : MonoBehaviour {

    public Texture2D Texture;
    public Material Mat;
	// Use this for initialization

	void Start () {
        MeshData meshData = MeshGenerator.GeneratePlaneMesh(64, 64);
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.material = Mat;
        meshRenderer.sharedMaterial.mainTexture = Texture;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
