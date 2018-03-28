using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Player
{
    public class PlayerData
    {
        #region singleton implementation
        public static PlayerData _instance;

        private PlayerData()
        {
            Health = 50;
            Gold   = 0;
        }

        public static PlayerData GetSharedInstance()
        {
            if (_instance == null)
            {
                _instance = new PlayerData();
            }
            return _instance;
        }
        #endregion

        public Transform PlayerWorldPosition;


        public int Health;
        public int Gold;


    }
}
