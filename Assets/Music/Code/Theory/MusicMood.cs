using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Music
{
    public class MusicMood
    {
        private static MusicMood _instance;

        public Mood World;
        public Mood Dungeon;
        public Mood Combat;


        public static MusicMood GetSharedInstance()
        {
            if(_instance == null)
            {
                _instance = new MusicMood();
                _instance.LoadMoods();
            }
            return _instance;
        }

        private void LoadMoods()
        {
            World = new Mood(KeyOf.G, 340, 0.6f, 5, 1, Theory.Scale_PentatonicMajor);
            World.LeadInstrumentSample = MusicMood.LoadSample("electric piano new c6");
            World.MelodyInstrumentSample = MusicMood.LoadSample("electric piano new c6");
            World.ASRSettings = new ASRSettings(0.67f, 0.9f, 0.58f, 0.1f, 0.377f, 0.18f);

            Dungeon = new Mood(KeyOf.Am, 300, 0.4f, 4, 1, Theory.Scale_HarmonicMinor);
            Dungeon.LeadInstrumentSample = MusicMood.LoadSample("piano-FM-octave0"); //piano fm oct 0
            Dungeon.MelodyInstrumentSample = MusicMood.LoadSample("violin_Cs5_05_mezzo-forte_arco-normal"); ; //violin c5 messo
            Dungeon.ASRSettings = new ASRSettings(0.885f, 1.09f, 0.888f, 0.55f, 1.21f, 0.63f);

            Combat = new Mood(KeyOf.C, 570, 0.89f, 10, 1, Theory.Scale_PentatonicMinor);
            Combat.LeadInstrumentSample = MusicMood.LoadSample("drum-bass-hi-2"); ; //drum bass hi-2
            Combat.MelodyInstrumentSample = MusicMood.LoadSample("mandolin-C-octave0"); ; //mandolin C octave 0
            Combat.ASRSettings = new ASRSettings(0.0f, 0.5f, 0.02f, 0.03f, 0.3f, 0.12f);
        }
        
        private static AudioClip LoadSample(string samplename)
        {
            AudioClip sample = (AudioClip)Resources.Load("Music/Samples/" + samplename);
            return sample;
        }
    }

    public class Mood {

        public KeyOf Key = KeyOf.C;
        public Note TonicNote;
        public int Tempo = 1;
        public float ChordChange = 0.0f;
        public int MaxMelodyNotes = 0;
        public int MinNoteSpacing = 1;

        //define instrument lists for mood
        public AudioClip LeadInstrumentSample;
        public AudioClip MelodyInstrumentSample;
        //define ASR envelope for mood
        public ASRSettings ASRSettings { get; set; }

        public List<int> ScaleIntervals;

        public Mood(KeyOf key, int tempo, float chordchange, int maxnotes, int minspace, List<int> moodscale)
        {
            Key = key;
            TonicNote = Theory.KeyTonicNote[Key];
            Tempo = tempo;
            ChordChange = chordchange;
            MaxMelodyNotes = maxnotes;
            MinNoteSpacing = minspace;
            ScaleIntervals = moodscale;
            ASRSettings = new ASRSettings(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    public class ASRSettings{

        public float Attack_Lead    { get; set; }
        public float Sustain_Lead   { get; set; }
        public float Release_Lead   { get; set; }

        public float Attack_Melody  { get; set; }
        public float Sustain_Melody { get; set; }
        public float Release_Melody { get; set; }

        public ASRSettings(float a_l, float s_l, float r_l, float a_m, float s_m, float r_m)
        {
            Attack_Lead = a_l;
            Sustain_Lead = s_l;
            Release_Lead = r_l;
            Attack_Melody = a_m;
            Sustain_Melody = s_m;
            Release_Melody = r_m;
        }
    }
}
