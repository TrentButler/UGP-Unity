using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class GamemodeManager : NetworkBehaviour
    {
        public Gamemode gamemode;

        private void Start()
        {
            if(isServer)
            {
                gamemode.Initialize();
            }
        }

        private void FixedUpdate()
        {
            if(isServer)
            {
                gamemode.GameLoop();

                if(gamemode.CheckWinCondition() || gamemode.WinCondition)
                {
                    gamemode.OnWinCondition();
                }

                if(gamemode.CheckLoseCondition() || gamemode.LoseCondition)
                {
                    gamemode.OnLoseCondition();
                }
            }
        }
    }

}