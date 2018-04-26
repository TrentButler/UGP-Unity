using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class TestGamemode : Gamemode
    {
        [SyncVar(hook = "OnTimerChange")] public float DebugTimer;
        
        private void OnTimerChange(float timerChange)
        {
            DebugTimer = timerChange;
        }

        public override bool CheckLoseCondition()
        {
            if(MatchTimer <= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CheckWinCondition()
        {
            return false;
        }

        public override void GameLoop()
        {
            DebugTimer -= Time.deltaTime;

            Debug.Log("MatchTimer: " + DebugTimer.ToString());
            Debug.Log("WinConditiion " + CheckWinCondition().ToString());
            Debug.Log("LoseConditiion " + CheckLoseCondition().ToString());
        }

        public override void Initialize()
        {
            DebugTimer = MatchTimer;
        }

        public override void OnWinCondition()
        {
            throw new System.NotImplementedException();
        }

        public override void OnLoseCondition()
        {
            throw new System.NotImplementedException();
        }

        public override void RestartPreMatchTimer()
        {
            throw new System.NotImplementedException();
        }
    }
}