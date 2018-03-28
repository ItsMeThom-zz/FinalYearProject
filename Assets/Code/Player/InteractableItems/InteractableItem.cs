using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Player
{
    /// <summary>
    /// Base component that all interactable items derive from
    /// Sets up a reference to the GameController, so children
    /// can refence game and player components at runtime.
    /// </summary>
    public class InteractableItem : MonoBehaviour
    {

        public string Name;
        public string Info;

        protected GameController GameController = GameController.GetSharedInstance();

        
    }
}

