using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// SamplerVoice is a container for audio output voices + filter envelopes
/// It will add a nessecary AudioSource when instantiated
/// </summary>
/// 
[RequireComponent(typeof(AudioSource))]
public class SamplerVoice : MonoBehaviour {

    private AudioSource _audioSource;

    private ASR_Envelope _asrEnvelope;

    public SamplerController ParentController;

    public float Pitch {
        get {
            return _audioSource.pitch;
        }
        set
        {
            _audioSource.pitch = value;
        }
    }

    //when the gameobject is created
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _asrEnvelope = gameObject.AddComponent<ASR_Envelope>();
        if(_asrEnvelope == null) { Debug.Log("No Envelope!"); }
    }


    public void Play(AudioClip audioClip, double attackTime, double sustainTime, double releaseTime)
    {
        //applying the AudioEnvelope to the audio data
        sustainTime = (sustainTime > attackTime) ? (sustainTime - attackTime) : 0.0;
        _asrEnvelope.Reset(attackTime, sustainTime, releaseTime, AudioSettings.outputSampleRate);
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    //Interface implementation to modify audiodata in an attached audiosource
    // This method called when AudioFilterRead event fires in child AudioSource
    private void OnAudioFilterRead(float[] buffer, int numChannels)
    {
        for (int sIdx = 0; sIdx < buffer.Length; sIdx += numChannels)
        {
            double volume = _asrEnvelope.GetLevel();

            for (int cIdx = 0; cIdx < numChannels; ++cIdx)
            {
                buffer[sIdx + cIdx] *= (float)volume;
            }
        }
    }
}
