using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public static class Utils
    {
        //list shuffle extention method
        // uses a seeded RNG for consistency
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, UnityEngine.Random.Range(i, list.Count));
        }

        //List quick swap extension method
        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }

        public static T Pop<T>(this IList<T> list)
        {
            T firstitem = list[0];
            list.RemoveAt(0);
            return firstitem;
        }



    }



}
