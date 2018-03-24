
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;


namespace Music
{
    /// <summary>
    /// Keeps time at a steady pace, and triggers a Tick() event each time a tick happens
    /// that other objects can subscribe to
    /// </summary>
    public class Metronome : MonoBehaviour
    {
        #region Public
        public event Action<double> Tick;
        public double bpm = 176; //Adagio Tempo 176
        public delegate void TickEvent();
        public static event TickEvent Ticked;

        #endregion
        #region Properties
        //Public property to access time to next tick
        public double NextTick { get { return nextTick; } }
        #endregion
        #region Private Variables
        private double nextTick = 0.0F; // The next tick in dspTime
        private double _tickLength; //tick length in seconds
        private double sampleRate = 0.0F;
        private bool ticked = false;
        #endregion


        void Start()
        {
            double startTick = AudioSettings.dspTime;
            sampleRate = AudioSettings.outputSampleRate;

            nextTick = startTick + (60.0 / bpm);
        }

        void LateUpdate()
        {
            if (!ticked && nextTick >= AudioSettings.dspTime)
            {
                ticked = true;
                if(Ticked != null)
                {
                    Ticked();
                }
            }
        }

        // Just an example OnTick here, can be in other child classes
        // in any subojects of this one, Using the BroadcastMessage function
        // to pass messages to all children
        //TODO: Use this in the future if the eventTick doesnt work out
        //void OnTick()
        //{
        //    //Debug.Log("Tick");
        //    Tick(nextTick);
        //
            // GetComponent<AudioSource>().Play();
       // }

        void FixedUpdate()
        {
            double timePerTick = 60.0f / bpm;
            double dspTime = AudioSettings.dspTime;

            while (dspTime >= nextTick)
            {
                ticked = false;
                nextTick += timePerTick;
            }

        }
    }

}
