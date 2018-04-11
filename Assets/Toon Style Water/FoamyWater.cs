using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FoamyWater : MonoBehaviour {

    public Texture2D foamTexture;

    private void Start()
    {
        Shader.SetGlobalTexture("_Foam", foamTexture);
        setFoamDynamics();
    }

    public bool stopTime = false;
    public Vector4 foamParameters;
    float MyTime = 0;
    public float _thickness = 80;
    public float _noise = 0.4f;
    public float _upscale = 1;
    public float shadowStrength = 1;
    public Color shadowColor;

    public void setFoamDynamics()  {
        Shader.SetGlobalVector("_foamDynamics", new Vector4(_thickness, _noise, _upscale, (300-_thickness)));
        shadowColor.a = 1-shadowStrength;
        Shader.SetGlobalColor("_ShadowColor", shadowColor);
    }


    // Update is called once per frame
    void Update () {
        if (!stopTime)
            MyTime += Time.deltaTime;

        foamParameters.x = MyTime; 
        foamParameters.y = MyTime * 0.6f;
        foamParameters.z = transform.position.y;

        Shader.SetGlobalVector("_foamParams", foamParameters);
      

	}
}
