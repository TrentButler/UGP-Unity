using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class VehicleDeathmatch : Gamemode
    {
        public InGameNetworkBehaviour server;
        public Text EndOfRaceText;
        public Text PreDeathmatchTimerText;

        public GameObject PreRaceTimerUI;
        public GameObject EndOfRaceUI;

        [SyncVar] private string _t;
        [SyncVar] private string _endofrace;
        [SyncVar(hook = "OnPreRaceTimerChange")] public float PreDeathmatchTimer;
        [SyncVar(hook = "OnPostRaceTimerChange")] public float PostDeathmatchTimer;
        [SyncVar(hook = "OnRaceTimerChange")] public float DeathmatchTimer;

        private void OnPreRaceTimerChange(float timerChange)
        {
            PreDeathmatchTimer = timerChange;
            PreDeathmatchTimer = Mathf.Clamp(PreDeathmatchTimer, 0.0f, PreMatchTimer);
        }
        private void OnPostRaceTimerChange(float timerChange)
        {
            PostDeathmatchTimer = timerChange;
            PostDeathmatchTimer = Mathf.Clamp(PostDeathmatchTimer, 0.0f, 999999);
        }
        private void OnRaceTimerChange(float timerChange)
        {
            DeathmatchTimer = timerChange;
        }

        [ClientRpc] private void RpcToggleEndOfRaceUI(bool toggle)
        {
            EndOfRaceUI.SetActive(toggle);
        }
        [ClientRpc] private void RpcTogglePreRaceTimerUI(bool toggle)
        {
            PreRaceTimerUI.SetActive(toggle);
        }

        private void ToggleEndOfRaceUI(bool toggle)
        {
            EndOfRaceUI.SetActive(toggle);
        }
        private void TogglePreRaceTimerUI(bool toggle)
        {
            PreRaceTimerUI.SetActive(toggle);
        }

        private void EndOfMatch()
        {
            PostDeathmatchTimer -= Time.deltaTime;
            if (PostDeathmatchTimer <= 0.0f)
            {
                Players.ForEach(player =>
                {
                    var network_identity = player.GetComponent<NetworkIdentity>();
                    server.RpcServer_Disconnect(network_identity, "");
                });

                var current_scene = SceneManager.GetActiveScene().name;
                server.Server_ChangeScene(current_scene);
            }

            _endofrace = "MATCH COMPLETE \n";
            _endofrace += "NEW MATCH IN: " + PostDeathmatchTimer.ToString() + "\n";
        }

        private void LateUpdate()
        {
            PreDeathmatchTimerText.text = "";
            PreDeathmatchTimerText.text = "BEGIN IN: " + PreDeathmatchTimer.ToString();
        }

        public override void OnWinCondition()
        {
            return;
        }

        public override void OnLoseCondition()
        {
            EndOfMatch();
            TogglePlayerControl(false);
            ToggleEndOfRaceUI(true);
            RpcToggleEndOfRaceUI(true);
        }

        public override bool CheckWinCondition()
        {
            return false;
        }

        public override bool CheckLoseCondition()
        {
            if (DeathmatchTimer <= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void GameLoop()
        {
            PreDeathmatchTimer -= Time.deltaTime;

            if (Players.Count <= 0)
            {
                RestartPreMatchTimer();
            }

            if (PreDeathmatchTimer > 0.0f)
            {
                TogglePlayerControl(false); //DISABLE PLAYER CONTROL

                PreRaceTimerUI.SetActive(true); //ENABLE THE PRERACETIMER UI
                RpcTogglePreRaceTimerUI(true);
            }
            else
            {
                TogglePlayerControl(true); //ENABLE PLAYER CONTROL

                PreRaceTimerUI.SetActive(false); //DISABLE THE PRERACETIMER UI
                RpcTogglePreRaceTimerUI(false);

                MatchBegun = true;

                DeathmatchTimer -= Time.deltaTime; //DECREMENT THE RACE TIMER
            }

            _t = "";
            _t += "Timer: " + DeathmatchTimer.ToString() + "\n";
        }

        public override void Initialize()
        {
            DeathmatchTimer = MatchTimer;
            PreDeathmatchTimer = PreMatchTimer;
            PostDeathmatchTimer = PostMatchTimer;
        }

        public override void RestartPreMatchTimer()
        {
            if (!MatchBegun)
            {
                PreDeathmatchTimer = PreMatchTimer;
            }
        }
    }
}