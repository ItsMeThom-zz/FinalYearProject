
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Music
{

    public class Note : IMusicalValue
    {
        //return the value to shift pitch of base note by.
        // ALWAYS assumes base note is a middle C.

        public Tone Value { get; set; }

        public Note(Tone value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return "Note: " + Value.ToString();
        }

    }
}
