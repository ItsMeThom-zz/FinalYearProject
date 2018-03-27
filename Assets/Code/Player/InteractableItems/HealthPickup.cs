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
            var player = PlayerData.GetSharedInstance();
            player.Health = (player.Health + Value > 100) ? 100: player.Health + Value;
            Destroy(this.gameObject);
        }

    }
}
