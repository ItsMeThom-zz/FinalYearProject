using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Music
{
    #region Enum Definitions
    //Denotes the pitch shift (scale^[i-1]);
    // Note that all values are 1 higher to account for C being I note/chord
    // -1 to play at that frequence
    public enum Tone { A = -2, As = -1, B = 0, C = 1, Cs = 2, D = 3, Ds = 4, E = 5, F = 6, Fs = 7, G = 8, Gs = 9, Rest }

    public enum NoteVal { A = -2, As = -1, B = 0, C = 1, Cs = 2, D = 3, Ds = 4, E = 5, F = 6, Fs = 7, G = 8, Gs = 9, Rest }
    public enum Numeral
    {
        I, II, III, IV, V, VI, VII, ii, iii, iv, v, vi, vii,
        i
    }
    public enum KeyOf { A, Am, B, Bm, C, Cm, D, Dm, E, Em, F, Fm, G, Gm }
    public enum ChordName
    {
        A, Am, As,
        B, Bm,
        C, Cm, Cs, Cms,
        D, Dm, Ds, Dms,
        E, Em, Es,
        F, Fm, Fs, Fms,
        G, Gm, Gs, Gms
    }
    public enum Scale { Major, Minor }
    public enum Harmonic { Third = 3, Fifth = 5 }
    #endregion

    public static class Theory
    {

        #region Note Definitions
        public static Note Rest = new Note(Tone.Rest);
        public static Note a = new Note(Tone.A);
        public static Note a_s = new Note(Tone.As);
        public static Note b = new Note(Tone.B);
        public static Note c = new Note(Tone.C);
        public static Note c_s = new Note(Tone.Cs);
        public static Note d = new Note(Tone.D);
        public static Note d_s = new Note(Tone.Ds);
        public static Note e = new Note(Tone.E);
        public static Note f = new Note(Tone.F);
        public static Note f_s = new Note(Tone.Fs);
        public static Note g = new Note(Tone.G);
        public static Note g_s = new Note(Tone.Gs);

        public static List<Note> AllNotes = new List<Note>()
        {
            Rest,
            a,
            a_s,
            b,
            c,
            c_s,
            d,
            d_s,
            e,
            f,
            f_s,
            g,
            g_s
        };

        /// <summary>
        /// Returns the note at the interval above the selected note (with wraparound)
        /// </summary>
        /// <param name="n">your note</param>
        /// <param name="interval">the number of steps above this note to go to</param>
        /// <returns></returns>
        public static Note GetNextNoteByInterval(Note n, int interval)
        {
            int noteIndex = AllNotes.IndexOf(n);
            int intervalIndex = (noteIndex + interval) % 12;
            if (intervalIndex < 0) { intervalIndex = 12 + intervalIndex; }
            Debug.Log("Note: " + noteIndex);
            Debug.Log("Interval: " + interval);
            Debug.Log("New Note: " + intervalIndex);
            return AllNotes[intervalIndex];
        }

        public static Note GetNote(Tone t)
        {
            // This is horrible code, thomas. Please fix this before
            // you rely on it too much and cant change it (again)
            switch (t)
            {
                case Tone.A:
                    return a;
                case Tone.B:
                    return b;
                case Tone.C:
                    return c;
                case Tone.Cs:
                    return c_s;
                case Tone.D:
                    return d;
                case Tone.Ds:
                    return d_s;
                case Tone.E:
                    return e;
                case Tone.F:
                    return f;
                case Tone.Fs:
                    return f_s;
                case Tone.G:
                    return g;
                case Tone.Gs:
                    return g_s;
                default:
                    return Rest;

            }
        }
        #endregion

        #region ScaleIntervals

        // intervals (offsets) from the root note of the key
        // defines which notes exist in that scale

        //atonal
        public static List<int> Scale_Chromatic = new List<int>() {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };
        // classic, happy
        public static List<int> Scale_Major = new List<int>() {
            2, 4, 5, 7, 9, 11, 12
        };
        // haunting, creepy
        public static List<int> Scale_HarmonicMinor = new List<int>() {
            2,3,5,7,8,11,12
        };
        //blues, rock
        public static List<int> Scale_PentatonicMinor = new List<int>() {
            3,5,7,10,12
        };
        //scary, epic
        public static List<int> Scale_NaturalMinor = new List<int>() {
            //2,1,2,2,1,2,2
            2,3,5,7,8,10,12
        };
        //wistful, mysterious
        public static List<int> Scale_MelodicMinorUp = new List<int>() {
            //2,1,2,2,2,2,1
            2,3,5,7,9,11,12
        };
        //sombre, soulful
        public static List<int> Scale_MelodicMinorDown = new List<int>() {
            //2,2,1,2,2,1,2
            2,4,5,7,9,10,12
        };
        //cool, jazzy
        public static List<int> Scale_Dorian = new List<int>() {
            //2,1,2,2,2,1,2
            2,3,5,7,9,10,12
        };
        //progressive, complex
        public static List<int> Scale_Mixolydian = new List<int>() {
            //2,2,1,2,2,1,2
            2,4,5,7,9,10,12
        };
        //exotic, unfamiliar
        public static List<int> Scale_AhavaRaba = new List<int>() {
            //1,3,1,2,1,2,2
            1,4,5,7,8,10,12
        };
        //country, gleeful
        public static List<int> Scale_PentatonicMajor = new List<int>() {
            //2,2,3,2,3
            2,4,7,9,12
        };
        //bizzare
        public static List<int> Scale_Diatonic = new List<int>() {
            //2,2,2,2,2,2
            2,4,6,8,10,12
        };

        public static List<int> ScaleIntervalSteps = new List<int>()
        {
            -1,-1,0,0,1,1,1,1,
        };
        #endregion

        #region Chord Definitions
        public static Chord C    = new Chord(new List<Tone>() { Tone.C, Tone.E, Tone.G });
        public static Chord C_s  = new Chord(new List<Tone>() { Tone.Cs, Tone.F, Tone.Gs});
        public static Chord D    = new Chord(new List<Tone>() { Tone.D, Tone.Fs, Tone.A });
        public static Chord D_s  = new Chord(new List<Tone>() { Tone.Ds, Tone.G, Tone.As });
        public static Chord E    = new Chord(new List<Tone>() { Tone.E, Tone.Gs, Tone.B });
        public static Chord F    = new Chord(new List<Tone>() { Tone.F, Tone.A, Tone.C });
        public static Chord F_s  = new Chord(new List<Tone>() { Tone.Fs, Tone.As, Tone.Cs });
        public static Chord G    = new Chord(new List<Tone>() { Tone.G, Tone.B, Tone.D });
        public static Chord G_s  = new Chord(new List<Tone>() { Tone.Gs, Tone.C, Tone.Ds });
        public static Chord A    = new Chord(new List<Tone>() { Tone.A, Tone.Cs, Tone.E });
        public static Chord A_s  = new Chord(new List<Tone>() { Tone.As, Tone.D, Tone.F });
        public static Chord B    = new Chord(new List<Tone>() { Tone.B, Tone.Ds, Tone.F });
        public static Chord C_m  = new Chord(new List<Tone>() { Tone.C, Tone.Ds, Tone.G });
        public static Chord C_ms = new Chord(new List<Tone>() { Tone.Cs, Tone.E, Tone.Gs });
        public static Chord D_m  = new Chord(new List<Tone>() { Tone.Ds, Tone.Fs, Tone.As });
        public static Chord E_m  = new Chord(new List<Tone>() { Tone.E, Tone.G, Tone.B });
        public static Chord F_m  = new Chord(new List<Tone>() { Tone.F, Tone.As, Tone.C });
        public static Chord F_ms = new Chord(new List<Tone>() { Tone.Fs, Tone.A, Tone.Cs });
        public static Chord G_m  = new Chord(new List<Tone>() { Tone.G, Tone.As, Tone.D });
        public static Chord G_ms = new Chord(new List<Tone>() { Tone.Gs, Tone.B, Tone.Ds });
        public static Chord A_m  = new Chord(new List<Tone>() { Tone.A, Tone.C, Tone.E });
        public static Chord B_m  = new Chord(new List<Tone>() { Tone.B, Tone.D, Tone.F });
        #endregion

        #region Key Chord Progression Graphs
        public static Dictionary<Numeral, ProgressionNode> ProgressionGraph_Major = new Dictionary<Numeral, ProgressionNode>()
            {
                // I chord
                {Numeral.I,
                    new ProgressionNode(
                    new List<Edge>(){
                        //new Edge(Numeral.I, 0.1f),
                        new Edge(Numeral.ii, 0.01f),
                        new Edge(Numeral.iii, 0.01f),
                        new Edge(Numeral.IV, 0.25f),
                        new Edge(Numeral.V, 0.25f),
                        new Edge(Numeral.vi, 0.01f),
                        new Edge(Numeral.vii, 0.01f),
                    }) },
                // ii chord
                {Numeral.ii,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.IV, 0.4f),
                        new Edge(Numeral.V, 0.4f),
                        new Edge(Numeral.vii, 0.05f),
                    })},
                // iii chord
                {Numeral.iii,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.IV, 1.0f),
                    })},
                // IV chord
                {Numeral.IV,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.I, 0.46f),
                        new Edge(Numeral.ii, 0.08f),
                        new Edge(Numeral.V, 0.6f),
                        //TODO V7 chords
                    })},
                // V chord
                {Numeral.V,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.I, 0.6f),
                        new Edge(Numeral.IV, 0.33f),
                        new Edge(Numeral.vi, 0.33f),
                    }) },
                // vi chord
                { Numeral.vi,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.IV, 0.4f),
                        new Edge(Numeral.ii, 0.4f),
                        new Edge(Numeral.V, 0.1f),
                    }) },
                // vii chord
                { Numeral.vii,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.I, 0.5f),
                        new Edge(Numeral.iii, 0.5f),
                    }) }
            };
        public static Dictionary<Numeral, ProgressionNode> ProgressionGraph_Minor = new Dictionary<Numeral, ProgressionNode>()
            {
                // i chord
                { Numeral.i,
                new ProgressionNode(
                    new List<Edge>(){
                        //new Edge(Numeral.i, 0.1f),
                        new Edge(Numeral.ii, 0.3f),
                        new Edge(Numeral.III, 0.4f),
                        new Edge(Numeral.iv, 0.25f),
                        new Edge(Numeral.v, 0.25f),
                        new Edge(Numeral.VI, 0.1f),
                        new Edge(Numeral.VII, 0.1f),
                    }) },
                // ii chord
                { Numeral.ii,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.v, 0.4f),
                        new Edge(Numeral.i, 0.2f), 
                        //viiD
                    }) },
               // iii chord
               { Numeral.III,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.iv, 0.4f),
                        new Edge(Numeral.VI, 0.4f),
                    }) },
               // iv chord
               { Numeral.iv,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.i, 0.5f),
                        new Edge(Numeral.ii, 0.08f),
                        new Edge(Numeral.v, 0.5f),
                        new Edge(Numeral.VII, 0.4f),
                        //TODO V7 chords
                    }) },
               // v chord
               { Numeral.v,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.i, 0.33f),
                    }) },
               // VI chord
               { Numeral.VI,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.iv, 0.4f),
                        new Edge(Numeral.ii, 0.4f),
                    }) },
                // VII chord
                { Numeral.VII,
                new ProgressionNode(
                    new List<Edge>(){
                        new Edge(Numeral.III, 1.0f),
                    }) }
            };
        #endregion

        public static Dictionary<KeyOf, Scale> KeyScaleType = new Dictionary<KeyOf, Scale>()
        {
            { KeyOf.A, Scale.Major},
            { KeyOf.B, Scale.Major},
            { KeyOf.C, Scale.Major},
            { KeyOf.D, Scale.Major},
            { KeyOf.E, Scale.Major},
            { KeyOf.F, Scale.Major},
            { KeyOf.G, Scale.Major},
            { KeyOf.Am, Scale.Minor},
            { KeyOf.Bm, Scale.Minor},
            { KeyOf.Cm, Scale.Minor},
            { KeyOf.Dm, Scale.Minor},
            { KeyOf.Em, Scale.Minor},
            { KeyOf.Fm, Scale.Minor},
            { KeyOf.Gm, Scale.Minor},

        };

        /// <summary>
        /// The root note of each key is needed for Determining the starting point for scales
        /// </summary>
        public static Dictionary<KeyOf, Note> KeyTonicNote = new Dictionary<KeyOf, Note>()
        {
            {KeyOf.A, a },
            {KeyOf.Am, a },
            {KeyOf.B, b },
            {KeyOf.Bm, b },
            {KeyOf.C, c },
            {KeyOf.Cm, c_s },
            {KeyOf.D, d },
            {KeyOf.Dm, d_s },
            {KeyOf.E, e },
            {KeyOf.Em, e },
            {KeyOf.F, f },
            {KeyOf.Fm, f },
            {KeyOf.G, g },
            {KeyOf.Gm, g },
        };

        public static List<List<Beat>> BeatPatterns = new List<List<Beat>>()
        {
            //new List<Beat>(){Beat.One},
            new List<Beat>(){Beat.One, Beat.Three},
            new List<Beat>(){Beat.Two, Beat.Four},
            new List<Beat>(){Beat.One, Beat.Three, Beat.Four}
        };

        #region Chords by Numeral Position in Key
        public static Chord GetChordOfKey(KeyOf k, Numeral i)
        {
            return ChordsByKey[k][i];
        }

        //TODO: No minor keys added yet. Need to add diminshed chords
        public static Dictionary<KeyOf, Dictionary<Numeral, Chord>> ChordsByKey = new Dictionary<KeyOf, Dictionary<Numeral, Chord>>()
        {
            {KeyOf.C, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   C},
                { Numeral.ii,  D_m},
                { Numeral.iii, E_m},
                { Numeral.IV,  F},
                { Numeral.V,   G},
                { Numeral.vi,  A_m},
                { Numeral.vii,  B},
            }},
            {KeyOf.D, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   D},
                { Numeral.ii,  E_m},
                { Numeral.iii, F_ms},
                { Numeral.IV,  G},
                { Numeral.V,   A},
                { Numeral.vi,  B_m},
                { Numeral.vii,  C_ms},
            }},
            {KeyOf.E, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   E},
                { Numeral.ii,  F_ms},
                { Numeral.iii, G_ms},
                { Numeral.IV,  A},
                { Numeral.V,   B},
                { Numeral.vi,  C_ms},
                { Numeral.vii, D_s} //dim
            }},
            {KeyOf.F, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   F},
                { Numeral.ii,  G},
                { Numeral.iii, A_m},
                { Numeral.IV,  B},
                { Numeral.V,   C},
                { Numeral.vi,  D_m},
                { Numeral.vii,  E_m} //dim
            }},
            {KeyOf.G, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   G},
                { Numeral.ii,  A_m},
                { Numeral.iii, B_m},
                { Numeral.IV,  C},
                { Numeral.V,   D},
                { Numeral.vi,  E_m},
                { Numeral.vii,  F_ms} //dim
            }},
            {KeyOf.A, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   A},
                { Numeral.ii,  B_m},
                { Numeral.iii, C_ms},
                { Numeral.IV,  D},
                { Numeral.V,   E},
                { Numeral.vi,  F_ms},
                { Numeral.vii,  G_ms} //dim
            }},
            {KeyOf.B, new Dictionary<Numeral, Chord>()
            {
                { Numeral.I,   B},
                { Numeral.ii,  C_ms},
                { Numeral.iii, D_m}, //Dms
                { Numeral.IV,  E},
                { Numeral.V,   F_s},
                { Numeral.vi,  G_ms},
                { Numeral.vii,  A_s} //dim
            }},
            {KeyOf.Am, new Dictionary<Numeral, Chord>()
            {
                { Numeral.i,   A_m},
                { Numeral.ii,  B},
                { Numeral.III, C},
                { Numeral.iv,  D_m},
                { Numeral.v,   E_m},
                { Numeral.VI,  F},
                { Numeral.VII,  G} //dim
            }},

        };
        #endregion
    }

    //internal class
    public class ProgressionNode
    {
        /// <summary>
        /// ChordNode is an internal structure that represents a weighted graph
        /// Returns a ChordName when Next() is called to simulate chord progression
        /// </summary>
        public Numeral Name { get; set; }
        private List<Edge> _edges;
        private float _totalWeight = 0f;

        public ProgressionNode(List<Edge> edges)
        {
            this._edges = edges;
            foreach (Edge e in _edges)
            {
                _totalWeight += e.Weight;
            }
        }
        //WEIGHTED GRAPH CHOICE
        //TODO: Move Generic Version of this to Util Class
        public Numeral Next()
        {
            Numeral selected = this.Name;
            // if we fail to make a selection, return this chord
            // totalWeight is the sum of all edges weight
            float randomNumber = UnityEngine.Random.Range(0f, _totalWeight);
            foreach (Edge edge in _edges)
            {
                if (randomNumber <= edge.Weight)
                {
                    selected = edge.Name;
                    break;
                }
                randomNumber = randomNumber - edge.Weight;
            }
            return selected;
        }
    }

    //internal class
    public class Edge
    {
        Numeral name;
        float weight;
        public Edge(Numeral name, float weight)
        {
            this.name = name;
            this.weight = weight;
        }
        public Numeral Name { get { return this.name; } }
        public float Weight { get { return this.weight; } }
    }
}

