using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBank : MonoBehaviour {

    [SerializeField]
    public List<AudioClip> Samples;
    [SerializeField]
    public AudioClip RestSample;
    private void Awake()
    {
        Samples = new List<AudioClip>();
        LoadSamples();
    }

    private void LoadSamples()
    {


    }
}
