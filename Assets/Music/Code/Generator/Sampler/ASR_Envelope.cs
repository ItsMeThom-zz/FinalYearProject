using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASR_Envelope : MonoBehaviour {

    private enum State
    {
        Idle,
        Attack,
        Sustain,
        Release
    }

    private State _state;
    private double _attackIncrement;
    private uint _sustainSamples;
    private double _releaseIncrement;
    private double _outputLevel;

    /// <summary>
    /// Reset the Envelope levels
    /// </summary>
    /// <param name="attackTime_s"></param>
    /// <param name="sustainTime_s"></param>
    /// <param name="releaseTime_s"></param>
    /// <param name="sampleRate"></param>
    public void Reset(double attackTime_s, double sustainTime_s, double releaseTime_s, int sampleRate)
    {
        _state = State.Attack;
        _attackIncrement = (attackTime_s > 0.0) ? (1.0 / (attackTime_s * sampleRate)) : 1.0;
        _sustainSamples = (uint)(sustainTime_s * sampleRate);
        _releaseIncrement = (releaseTime_s > 0.0) ? (1.0 / (releaseTime_s * sampleRate)) : 1.0;
        _outputLevel = 0.0;
    }

    /// <summary>
    /// Get current outpu volume as defined by the sample position in the envelope
    /// Sets the current envelope state (Attack, Sustain, Release)
    /// </summary>
    /// <returns>double outputLevel</returns>
    public double GetLevel()
    {
        switch (_state)
        {
            case State.Idle:
                _outputLevel = 0.0;
                break;
            case State.Attack:
                _outputLevel += _attackIncrement;

                if (_outputLevel > 1.0)
                {
                    _outputLevel = 1.0;
                    _state = State.Sustain;
                }

                break;
            case State.Sustain:
                if ((_sustainSamples == 0) || (--_sustainSamples == 0))
                {
                    _state = State.Release;
                }

                break;
            case State.Release:
                _outputLevel -= _releaseIncrement;

                if (_outputLevel < 0.0)
                {
                    _outputLevel = 0.0;
                    _state = State.Idle;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return _outputLevel;
    }
}
