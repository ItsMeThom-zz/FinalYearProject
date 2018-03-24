
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
        Queue<Atom> AtomQueue;

        private void Awake()
        {
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
            }
           
        }
        
        public void LoadAtomPrefab(Transform replaceTransform, string path)
        {
            Debug.Log("GE loading prefab: grammar/" + path);
            GameObject newAtom = (GameObject)Resources.Load("grammar/" + path);
            
            GameObject spawnedObject = Instantiate( newAtom, 
                                                    replaceTransform.TransformPoint(Vector3.zero), 
                                                    replaceTransform.rotation, 
                                                    gameObject.transform.parent);
            Atom spawnedAtom = spawnedObject.GetComponentInChildren<Atom>();
            spawnedAtom.transform.parent = replaceTransform.parent;
            if(spawnedAtom.SubAtoms != null)
            {
                foreach (var subatom in spawnedAtom.SubAtoms)
                {
                    subatom.GrammarEngine = this;
                    AtomQueue.Enqueue(subatom);
                }
            }
            Destroy(replaceTransform.gameObject);
        }

    }
}
