using Music;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SamplerController manages SampleVoice containers for AudioClip playback
/// </summary>
public class SamplerController : MonoBehaviour {

    [Header("Config")]
    // The audio file we want to play
    [SerializeField]
    public AudioClip LeadInstrument;
    public AudioClip MelodyInstrument;
    public AudioClip RestSpacer;
    // The number of voices. Set this higher if there is excessive voice stealing.
    [SerializeField, Range(1, 48)] private int _numVoices = 48;

    [SerializeField, Range(0f, 2f)] private double _attackLead = 0.5f;
    [SerializeField, Range(0f, 2f)] private double _sustainLead = 1.5f;
    [SerializeField, Range(0f, 2f)] private double _releaseLead = 0.5f;

    [SerializeField, Range(0f, 2f)] private double _attackMelody = 0.5f;
    [SerializeField, Range(0f, 2f)] private double _sustainMelody = 1.0f;
    [SerializeField, Range(0f, 2f)] private double _releaseMelody = 1.5f;

    public SamplerVoice SamplerVoicePrefab;
    private SamplerVoice[] _samplerVoices;

    // The index of the next voice to play
    private int _nextVoiceIndex;

    /// <summary>
    /// This gets called when the GameObject first gets created.
    /// Create our sampler voices here.
    /// </summary>
    private void Start()
    {

        _samplerVoices = new SamplerVoice[_numVoices];

        for (int i = 0; i < _numVoices; ++i)
        {
            SamplerVoice samplerVoice = Instantiate(SamplerVoicePrefab);
            samplerVoice.transform.parent = transform;
            samplerVoice.ParentController = this;
            samplerVoice.transform.localPosition = Vector3.zero;
            samplerVoice.name = "Sample Voice";
            _samplerVoices[i] = samplerVoice;
        }
        _nextVoiceIndex = 0;
        
    }

    /// <summary>
    /// Pick a voice and play the sound
    /// </summary>
    public void Play(AudioClip clip)
    {
        // Play the sound on the next voice (And pass envelope modifications)
        _samplerVoices[_nextVoiceIndex].Play(clip, _attackLead, _sustainLead, _releaseLead);
        ++_nextVoiceIndex;
        //wrap nextvoice around if it exceeds max
        _nextVoiceIndex = (_nextVoiceIndex + 1) % _samplerVoices.Length;
    }

    /// <summary>
    /// Passed a note/chord for lead, and note for melody. This chooses a sampler
    /// on which to play the note, sets the pitch accordingly, and then plays the single note
    /// or in the case of a lead chord, it plays all notes of the chord
    /// 
    /// scale is the 12th root of 2, or 1 semitone step, the basis assumes its at C (first note) of
    /// the scale. It is precomputed, and reset each time a note tries to play to prevent weird offsets
    /// 
    /// FUTURE WORK: Precompute all scale^notevals and use that instead of Math.Pow()
    /// </summary>
    public void Play(IMusicalValue leadValue, IMusicalValue melodyValue)
    {
        float scale = 1.059463f;// Mathf.Pow(2f, 1.0f / 12f);
        // Play the sound on the next voice (And pass envelope modifications)

        //Base chords and individual notes for lead
        if (leadValue is Chord)
        {
            Chord leadChord = leadValue as Chord;
            //Debug.Log("Chord Cast");
            foreach (Tone note in leadChord.Notes)
            {
                _samplerVoices[_nextVoiceIndex].Pitch = Mathf.Pow(scale, (float)note - 1);
                _samplerVoices[_nextVoiceIndex].Play(LeadInstrument, _attackLead, _sustainLead, _releaseLead);
                ++_nextVoiceIndex;
                _nextVoiceIndex = (_nextVoiceIndex + 1) % _samplerVoices.Length;
            }
            
        }else if(leadValue is Note)
        {
            //Playing a single note
            
            Note leadNote = leadValue as Note;
            _samplerVoices[_nextVoiceIndex].Pitch = Mathf.Pow(scale, (float)leadNote.Value - 1);
            if (leadNote.Value == Tone.Rest)
            {
                _samplerVoices[_nextVoiceIndex].Play(RestSpacer, _attackLead, _sustainLead, _releaseLead);
            }
            else
            {
                //Debug.Log("Note Cast and played");
                _samplerVoices[_nextVoiceIndex].Play(LeadInstrument, _attackLead, _sustainLead, _releaseLead);
            }
            ++_nextVoiceIndex;
            _nextVoiceIndex = (_nextVoiceIndex + 1) % _samplerVoices.Length;
            
        }

        if (melodyValue is Note)
        {
            //Playing a single note
            Note melodyNote = melodyValue as Note;
            _samplerVoices[_nextVoiceIndex].Pitch = Mathf.Pow(scale, (float)melodyNote.Value - 1);
            //Debug.Log("playing melody note" + melodyNote.Value);
            if(melodyNote.Value == Tone.Rest)
            {
                _samplerVoices[_nextVoiceIndex].Play(RestSpacer, _attackMelody, _sustainMelody, _releaseMelody);
            }
            else
            {
                _samplerVoices[_nextVoiceIndex].Play(MelodyInstrument, _attackMelody, _sustainMelody, _releaseMelody);
            }
            ++_nextVoiceIndex;
            _nextVoiceIndex = (_nextVoiceIndex + 1) % _samplerVoices.Length;
            
        }
        //wrap nextvoice around if it exceeds max

    }

    public void SetASR(ASRSettings asr)
    {
        this._attackLead = asr.Attack_Lead;
        this._sustainLead = asr.Sustain_Lead;
        this._releaseLead = asr.Release_Lead;
        this._attackMelody = asr.Attack_Melody;
        this._sustainMelody = asr.Sustain_Melody;
        this._releaseMelody = asr.Release_Melody;
    }
}
