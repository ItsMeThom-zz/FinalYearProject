using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeGrammar
{
    /// <summary>
    /// An atom is attached to a gameobject whose children
    /// are a set of props/prefabs prearranged in a specific order
    /// Atoms are made into prefabs and instantiated
    /// which calls Awake()
    ///     - gathers all child props into list.
    /// and then Start()
    ///     - attempts to rewrite itself as needed
    /// 
    /// </summary>
    ///
    [Serializable]
    [ExecuteInEditMode]
    public class Atom : MonoBehaviour
    {
        //Editor Drawing Mode
        [SerializeField]
        public bool DrawLayout = true;
        [SerializeField]
        public float Width = 10f;
        [SerializeField]
        public float Height = 10f;
        [SerializeField]
        public float Depth = 0.0001f;
        [SerializeField]
        public Color LinesColor = Color.red;

        public GrammarEngine GrammarEngine { get; set; }

        [SerializeField]
        public List<GrammarRule> RuleSet = new List<GrammarRule>(); //protects editmode shennanigans



        public List<GameObject> Props { get; set; }
        public List<Atom> SubAtoms { get; set; }

       
        //  initalise rules/props as needed
        private void Awake()
        {


            if(this.RuleSet == null)
            {
                this.RuleSet = new List<GrammarRule>();

            }
            SubAtoms = new List<Atom>();
            GetSubAtoms();
            
        }

        public void Rewrite()
        {

            
            //iterate over list.
            // if value < weight
            // select that item.
            // tell GrammarEngine to instantiate it passing this transform.position for location
            // destory self
            if((this.RuleSet != null) && (this.RuleSet.Count > 0))
            {
                float totalWeight = GetTotalWeight();
                float randomNumber = UnityEngine.Random.Range(0f, totalWeight);

                GrammarRule selectedRule = RuleSet[0];
                foreach (GrammarRule rule in RuleSet)
                {
                    if (randomNumber <= rule.Weight)
                    {
                        selectedRule = rule;
                        break;
                    }
                    randomNumber = randomNumber - rule.Weight;
                }
                if(selectedRule.Type != null)
                {
                    //Create a new object at the atom components position
                    GameObject newAtom = Instantiate(selectedRule.Type, this.transform.position, this.transform.rotation);
                    
                    newAtom.transform.SetPositionAndRotation(this.transform.parent.position, Quaternion.identity);
                    newAtom.transform.SetParent(this.transform.parent, true);
                    var subatoms = newAtom.GetComponentsInChildren<Atom>();
                    foreach(var sb in subatoms)
                    {
                        Debug.Log("SB");
                        this.GrammarEngine.AtomQueue.Enqueue(sb);
                    }
                    //Destroy(this.gameObject);
                    
                }
                else
                {
                    Debug.Log("Grammar rule chose to rewrite but has no object prefab specified");
                }
                
           

            }

        }

        private void OnDrawGizmos()
        {

            if (DrawLayout)
            {
                Gizmos.color = LinesColor;
                Gizmos.DrawCube(this.transform.position, new Vector3(Width, Depth, Height));
            }
            
        }

        private float GetTotalWeight()
        {
            float totalWeight = 0.0f;
            foreach(var rule in RuleSet)
            {
                totalWeight += rule.Weight;
            }
            return totalWeight;
        }

        private void GetSubAtoms()
        {
            Atom[] subatoms = this.gameObject.GetComponentsInChildren<Atom>();
            Debug.Log("This atom [" + gameObject.name + "]" + " has " + subatoms.GetLength(0) + " subatoms");
            //the spawned item has subatoms we will need to resolve
            if (subatoms != null)
            {
                //We need to ignore this component in the list of returned components
                // as GetComponentsInChildren returns the parent component too! (wtf)
                for (int i = 0; i < subatoms.GetLength(0); i++)
                {
                    if (!subatoms[i].GetHashCode().Equals(this.GetHashCode()))
                    {
                        SubAtoms.Add(subatoms[i]);
                    }
                }

            }
        }
    }

}

