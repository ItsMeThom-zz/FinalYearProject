using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour {

    public float minLightIntensity = 0.5f;
    public float maxLightIntensity = 1.2f;
    public float minVariationFactor = 0.9f; //1.0 => no variation
    public float maxVariationFactor = 1.1f; //...
    public float speedFactor = 1.4f; //1.0 => normal speed
    private float variationFactor;
     
    void Start()
    {
        FixedUpdate();
    }

    void FixedUpdate()
    {
        // Slightly randomize the effect every deltaTime to create a flicker effect
        variationFactor = Random.Range(minVariationFactor, maxVariationFactor);
    }

    void Update()
    {
        // Set the intensity by fading between min- and maxLightIntensity,
        // while also applying speedFactor and variationFactor to the mix
        gameObject.GetComponent<Light>().intensity = (minLightIntensity
            + Mathf.PingPong(Time.time * speedFactor, maxLightIntensity)) * variationFactor;
    }

}
