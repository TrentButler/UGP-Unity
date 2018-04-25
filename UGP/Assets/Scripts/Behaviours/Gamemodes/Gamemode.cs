using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public abstract class Gamemode : NetworkBehaviour
    {
        //WHAT EVERY GAMEMODE WILL CONSIST OF
        //PRE-MATCH TIMER
        //MATCH TIMER
        //COLLECTION OF ALL CONNECTED PLAYERS

        [Range(0, 999999)] public float PreMatchTimer;
        [Range(0, 999999)] public float PostMatchTimer;
        [Range(0, 999999)] public float MatchTimer;
        public List<PlayerBehaviour> Players = new List<PlayerBehaviour>();
        public bool WinCondition;
        public bool LoseCondition;
        [SyncVar(hook = "OnMatchBegunChange")] public bool MatchBegun;

        private void OnMatchBegunChange(bool begunChange)
        {
            MatchBegun = begunChange;

            if(MatchBegun)
            {
                TogglePlayerControl(true);
            }
            else
            {
                TogglePlayerControl(false);
            }
        }

        public abstract bool CheckWinCondition();
        public abstract bool CheckLoseCondition();
        public abstract void OnWinCondition();
        public abstract void OnLoseCondition();
        public abstract void GameLoop();
        public abstract void Initialize();
        public abstract void RestartPreMatchTimer();

        protected void SetMatchBegun(bool begun)
        {
            MatchBegun = begun;
        }
        protected void TogglePlayerControl(bool control)
        {
            Players.ForEach(player =>
            {
                player.RpcSetUserControl(control);
            });
        }
    }
}