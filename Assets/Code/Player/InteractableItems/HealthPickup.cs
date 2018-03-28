using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Code.Player;

namespace Player.InteractableItems
{
    public class HealthPickup : InteractableItem, IInteractable
    {
   

        public int Value = 10;
        private void Awake()
        {
            this.Name = "Tasty Health Thing";
            this.Info = "+10 hp";
        }

        public void Consume(GameObject obj)
        {
            GameController.PlayerController.AddHealth(Value);
            Destroy(this.gameObject);
        }

    }
}
