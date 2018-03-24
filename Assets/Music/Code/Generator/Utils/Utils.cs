using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Music
{
    public static class Utils
    {

        public static T ChooseEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        public static T ChooseList<T>(List<T> list){
            var i = UnityEngine.Random.Range(0, list.Count - 1);
            return list[i];
        }

        public static bool FlipCoin()
        {
            var i = UnityEngine.Random.Range(0, 1);
            if(i == 0) { return false; }
            return true;
        }
    }
}
