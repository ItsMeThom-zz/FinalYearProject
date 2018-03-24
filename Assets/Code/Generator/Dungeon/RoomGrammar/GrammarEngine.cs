
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GenerativeGrammar
{
    public class GrammarEngine : MonoBehaviour
    {

        /*
         This is a component spawned after dungeon room generation
         It is added to the dungeon generator controller
         it initalises by finding all child Atoms in the dungeon generator controller children
         it gives each a reference to itself.
         it adds each to a queue
         it pops an item from the queue and rewrite it
         it does this until the queue is empty
         */
        public Queue<Atom> AtomQueue;

        private void Awake()
        {
            //Get all inital parent level atoms
            AtomQueue = new Queue<Atom>();
            Atom[] childAtoms = GetComponentsInChildren<Atom>();
            if( (childAtoms != null) && (childAtoms.Count() > 0))
            {
                for (int i = 0; i < childAtoms.Count(); i++)
                {
                    childAtoms[i].GrammarEngine = this;
                    AtomQueue.Enqueue(childAtoms[i]);
                }
            }
            else
            {
                Debug.Log("Grammar Engine found no child atoms");
            }
        }

        private void Start()
        {
            RewriteAtoms();
        }
        public void RewriteAtoms()
        {
            
            while(AtomQueue.Count > 0)
            {
                Debug.Log(AtomQueue.Count + " remaining rewrites");
                Atom atom = AtomQueue.Dequeue();
                atom.Rewrite();
                var childAtoms = atom.SubAtoms;
                Debug.Log("Rewritten Atom has " + childAtoms.Count + "atoms");
                foreach(var subatom in childAtoms)
                {
                    AtomQueue.Enqueue(subatom);
                }
            }
           
        }

        public GameObject RewriteSpecificAtom(GameObject starterObject)
        {
            var objectatom = starterObject.GetComponent<Atom>();
            if(objectatom != null)
            {
                AtomQueue.Clear();
                AtomQueue.Enqueue(objectatom);
                while(AtomQueue.Count > 0)
                {
                    Atom atom = AtomQueue.Dequeue();
                    atom.Rewrite();
                    var childAtoms = atom.SubAtoms;
                    
                    foreach (var subatom in childAtoms)
                    {
                        AtomQueue.Enqueue(subatom);
                    }
                }
            }
            return starterObject;
        }
    }
}
