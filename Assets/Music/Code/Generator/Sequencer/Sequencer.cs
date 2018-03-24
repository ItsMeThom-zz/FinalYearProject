using Music.Teree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace Music
{
    /// <summary>
    /// Provides an iterative method for stepping through a set of Track objects
    /// and decoding/playing the audiosamples in them
    /// </summary>
    
    [RequireComponent(typeof(SamplerController))]
    class Sequencer : MonoBehaviour
    {
        public SamplerController Sampler { get; set; }

        public MusicGenerator Generator { get; set; }

        private Mood currentMood;
        //Tracks contain all the notes we need to play
        Queue<IMusicalValue> LeadTrack;
        Queue<IMusicalValue> MelodyTrack;

        void Awake()
        {
            LeadTrack = new Queue<IMusicalValue>();
            MelodyTrack = new Queue<IMusicalValue>();
            Generator = MusicGenerator.GetInstance();
            Sampler = GetComponent<SamplerController>();
            //enqueue starting tracks
            Generator.Generate();
            GetNextTracks();
            //subscribe to the metronome tick event so we can keep time!
            Metronome.Ticked += Step;

        }

        /// <summary>
        /// Step each track forward one position and pass the notevalues to the sampler
        /// at that position
        /// </summary>
        public void Step()
        {
            IMusicalValue leadValue = LeadTrack.Dequeue();
            IMusicalValue melodyValue = MelodyTrack.Dequeue();
            Sampler.Play(leadValue, melodyValue);
            if(LeadTrack.Count < 2)
            {
                //Changing mood at some point within this
                MusicGenerator MusicGen = MusicGenerator.GetInstance();
                Metronome metro = FindObjectOfType<Metronome>();
                if (MusicGen.CurrentMood != currentMood)
                {
                    currentMood = MusicGen.CurrentMood;
                    var emptyBar = MusicGen.GetBar();
                    foreach (IMusicalValue note in emptyBar)
                    {
                        LeadTrack.Enqueue(note);
                        MelodyTrack.Enqueue(note);
                    }
                    
                    metro.bpm = currentMood.Tempo;
                    Sampler.LeadInstrument = currentMood.LeadInstrumentSample;
                    Sampler.MelodyInstrument = currentMood.MelodyInstrumentSample;
                    Sampler.SetASR(currentMood.ASRSettings);
                }
                //else
                //{
                //    //slow down as we fade out
                //    metro.bpm /= 2;
                //}
                GetNextTracks();
                
            }
          
        }

        /// <summary>
        /// Requests a new set of musical values for lead and melody and
        /// adds them to the current track(queue)
        /// </summary>
        public void GetNextTracks()
        {
            List<IMusicalValue> newLeadTrack   = Generator.GetLeadTrack();
            List<IMusicalValue> newMelodyTrack = Generator.GetMelodyTrack();
            if(newLeadTrack.Count != newMelodyTrack.Count)
            {
                Debug.Log("Lead and Melody Tracks are of differnt lengths!");
            }
            else
            {
                for(int i = 0; i < newLeadTrack.Count; i++)
                {
                    LeadTrack.Enqueue(newLeadTrack[i]);
                    MelodyTrack.Enqueue(newMelodyTrack[i]);
                }
            }
        }

    }
}
