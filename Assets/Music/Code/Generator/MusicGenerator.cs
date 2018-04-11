using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Music
{
    public enum Beat { One = 0, Two = 4, Three = 8, Four = 12}
    /// <summary>
    /// Music Generator is a shared singleton object
    /// </summary>
    public class MusicGenerator
    {
        private static MusicGenerator _instance;
        private MusicMood MusicMood;// = //MusicMood.GetSharedInstance();
        private GameController GameController = GameController.GetSharedInstance();
        public Mood CurrentMood;
        public static bool FirstPass = true;
        List<IMusicalValue> CurrentLead;
        List<IMusicalValue> CurrentMelody;

        List<List<int?>> CurrentMelodyPatterns;

        #region Track specifics
        int BarSize = 16; //subdivisions (quarternotes)
        List<Beat> BeatPattern;
        #endregion

        public static MusicGenerator GetInstance()
        {
            if(_instance == null)
            {
                
                _instance = new MusicGenerator();
                _instance.CurrentLead = new List<IMusicalValue>();
                _instance.CurrentMelody = new List<IMusicalValue>();
                _instance.CurrentMelodyPatterns = new List<List<int?>>();
                _instance.CurrentMood = ScriptableObject.CreateInstance<MusicMood>().World;
                _instance.GameController.MusicGenerator = _instance;
            }
            return _instance;
        }

        public void GetMood()
        {
            //test mood
            if (GameController.PlayerInCombat)
            {
                Debug.Log("COMBAT");
                CurrentMood = ScriptableObject.CreateInstance<MusicMood>().Combat;
            }else if (GameController.PlayerInDungeon)
            {
                Debug.Log("DUNGEON");
                //CurrentMood = MusicMood.Dungeon;
                CurrentMood = ScriptableObject.CreateInstance<MusicMood>().Dungeon;
            }
            else
            {
                Debug.Log("WORLD");
                //CurrentMood = MusicMood.World;
                CurrentMood = ScriptableObject.CreateInstance<MusicMood>().World;
            }
            //Debug.Log(CurrentMood.Key);
        }

        public void Generate()
        {
            GetMood();
            CurrentLead.Clear();
            CurrentMelody.Clear();
            var progressionGraph = GetProgressionTypeForKey(CurrentMood.Key);
            
            //choose lead progression
            Numeral tonicNumeral = Numeral.I;
            if (!progressionGraph.ContainsKey(tonicNumeral))
            {
                tonicNumeral = Numeral.i; //this is a minor key
            };
            List<Numeral> chosenProgression = new List<Numeral>()
            {
                tonicNumeral
            };
            var node = progressionGraph[tonicNumeral];
            int chordrange = UnityEngine.Random.Range(3, 6);
            for (int i = 0; i < chordrange; i++)
            {
                var nextChord = node.Next();
                chosenProgression.Add((nextChord));
                if (!progressionGraph.ContainsKey(nextChord))
                {
                    Debug.Log("Couldn't get " + nextChord + " in progressionGraph");
                }
                node = progressionGraph[nextChord];
            }

            List<IMusicalValue> generated = new List<IMusicalValue>();
            BeatPattern = ChooseBeatPattern();
            CurrentMelodyPatterns = GenerateMelodyPatterns(4);

            int currentChord = 0;
            //generate a phrase of 16 bars
            for(int i = 0; i < BarSize; i++)
            {
                var bar = GetBar();
                foreach(var beat in BeatPattern)
                {
                    //determine if we change chords on this beat
                    if (ChangeOnBeat())
                    {
                        //Debug.Log("Changing Chord..");
                        currentChord++;
                        if(currentChord >= chosenProgression.Count)
                        {
                            
                            currentChord = 0;
                        }
                    }
                    Chord chord = Theory.GetChordOfKey(CurrentMood.Key, chosenProgression[currentChord]);
                    //determine if we appregiate or not
                    if (Appregiate())
                    {
                        var x = (int)beat;
                        //appregio pattern
                        // 1 2 3
                        var pattern = UnityEngine.Random.value;
                        if (pattern <= 0.8)
                        {
                            foreach (var note in chord.Notes)
                            {
                                bar[x] = Theory.GetNote(note);
                                x++;
                            }
                        }
                        else
                        {
                            // 1 3 2
                            bar[x] = Theory.GetNote(chord.Notes[0]);
                            bar[x + 1] = Theory.GetNote(chord.Notes[2]);
                            bar[x + 2] = Theory.GetNote(chord.Notes[1]);
                        }
                    }
                    else
                    {
                        bar[(int)beat] = chord;
                    }
                }
                generated.AddRange(bar);
                //pick a reuseable melody pattern for here
                var melodyPattern = Utils.ChooseList(CurrentMelodyPatterns);
                var melodyBar = GetMelodyBar(bar, melodyPattern);
                CurrentMelody.AddRange(melodyBar);
            }

            CurrentLead = generated;

            FirstPass = false;
        }

        #region Generation helper methods
        public List<Beat> ChooseBeatPattern()
        {
            return Music.Utils.ChooseList<List<Beat>>(Theory.BeatPatterns);
        }
        public bool ChangeOnBeat()
        {
            return (UnityEngine.Random.value >= CurrentMood.ChordChange);
        }
        public bool Appregiate()
        {
            return (UnityEngine.Random.value > 0.6f);
        }

        //returns a list of nullable ints to build a melody pattern on
        private List<int?> GetBlankPatternBar()
        {
            List<int?> blankBar = new List<int?>();
            for(int i = 0; i < this.BarSize; i++)
            {
                blankBar.Add(null);
            }
            return blankBar;
        }

        /// <summary>
        /// Generates a Melody using the notes in this bar (lead) and a melody pattern
        /// </summary>
        private List<IMusicalValue> GetMelodyBar(List<IMusicalValue> bar, List<int?> pattern)
        {
            IMusicalValue barChord = bar[(int)Beat.One];
            Note rootBarNote = CurrentMood.TonicNote;
            if (barChord is Chord)
            {
                rootBarNote = Theory.GetNote(((Chord)barChord).Notes[0]);
            }
            else if (barChord is Note)
            {
                rootBarNote = (Note)barChord;
                if (rootBarNote.Value == Tone.Rest)
                {
                    rootBarNote = CurrentMood.TonicNote;
                }
            }
            else
            {
                Debug.Log("Cannot determine the root note or chord for this bar. Shouldnt happen.");
            }
            List<IMusicalValue> newMelodyBar = new List<IMusicalValue>();
            for (int n = 0; n < pattern.Count; n++)
            {
                if (pattern[n] != null)
                {
                    newMelodyBar.Add(Theory.GetNextNoteByInterval(rootBarNote, (int)pattern[n]));
                }
                else
                {
                    newMelodyBar.Add(Theory.Rest);
                }
            }
            return newMelodyBar;
        }
         

        private List<List<int?>> GenerateMelodyPatterns(int number)
        {
            List<List<int?>> patterns = new List<List<int?>>();
            //TODO: move this somewhere else
            List<int> stepSizes = new List<int>() { 1, 1, 1, 1, 1, 0, 0, 2, 2, -1, -1 };

            for (int num = 0; num < number; num++){
                List<int> patternValues = new List<int>();
                int totalNotes = UnityEngine.Random.Range(1, CurrentMood.MaxMelodyNotes + 1);
                int currentPos = UnityEngine.Random.Range(0, CurrentMood.ScaleIntervals.Count);
                for (int i = 0; i < totalNotes; i++)
                {
                    int step = Utils.ChooseList(stepSizes);
                    if (step + currentPos >= CurrentMood.ScaleIntervals.Count)
                    {
                        currentPos -= step; //step down
                    }
                    else if(step + currentPos <= 0)
                    {
                        currentPos = 0;
                    }
                    else
                    {
                        currentPos += step;
                    }
                    patternValues.Add(CurrentMood.ScaleIntervals[currentPos]);
                }
                List<int?> patternBar = GetBlankPatternBar();
                int pos = (int)Beat.Two;
                foreach (var note in patternValues)
                {
                    int noteStep = UnityEngine.Random.Range(0, 3);
                    if (pos + noteStep >= patternBar.Count) { break; }
                    patternBar[pos + noteStep] = note;
                    pos += CurrentMood.MinNoteSpacing;
                }
                patterns.Add(patternBar);
            }
            return patterns;
        }
        #endregion

        /// <summary>
        /// Returns the correct chord progression graph (maj/min) 
        /// depending on the chosen key. Defaults to Maj
        /// </summary>
        /// <param name="key">Chosen Musical Key</param>
        /// <returns></returns>
        private Dictionary<Numeral, ProgressionNode> GetProgressionTypeForKey(KeyOf key)
        {
            Scale scale = Scale.Major;
            Music.Theory.KeyScaleType.TryGetValue(key, out scale);
            Dictionary<Numeral, ProgressionNode> dict =
                (scale == Scale.Major) ? 
                Theory.ProgressionGraph_Major :
                Theory.ProgressionGraph_Minor;
            return dict;
        }

        public List<IMusicalValue> GetLeadTrack()
        {
            //Debug.Log("Current lead size = " + CurrentLead.Count);
            return CurrentLead;
        }

        public List<IMusicalValue> GetMelodyTrack()
        {
            return CurrentMelody;
        }

        public List<IMusicalValue> GetBar()
        {
            List<IMusicalValue> list = new List<IMusicalValue>();
            for(int i =0; i < 16; i++)
            {
                list.Add(Music.Theory.Rest);
            }
            return list;
        }

    }
}
