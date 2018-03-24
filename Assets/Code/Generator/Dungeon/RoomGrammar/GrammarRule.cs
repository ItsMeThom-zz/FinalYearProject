using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GenerativeGrammar
{

    [ExecuteInEditMode]
    [Serializable]
    public class GrammarRule
    {
        public GameObject Type;      // type to return if this rule is selected
        public float Weight = 0.0f;
        //public string Rewrite;

        public GrammarRule()
        {
            Type = null;
            Weight = 0.0f;
            //Rewrite = "None";
        }
    }
}
