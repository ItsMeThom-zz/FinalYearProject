using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code.TreeGen
{
    public class NormalThief : MonoBehaviour
    {
        void OnCollisionEnter(Collision other)
        {
           print("First normal of the point that collide: " + other.contacts[0].normal);
        }
    }
}
