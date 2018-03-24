using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Music
{
    public class Chord : IMusicalValue
    {
        public List<Tone> Notes { get; set; }

        public Chord(List<Tone> notes)
        {
            Notes = notes;
        }

        public override string ToString()
        {
            return "Chord: [" + Notes[0] + ", " + Notes[1] + ", " + Notes[2] + "]";
        }

    }
}
