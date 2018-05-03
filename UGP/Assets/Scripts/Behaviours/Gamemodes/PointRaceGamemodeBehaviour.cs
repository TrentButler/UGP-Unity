using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class PointRaceGamemodeBehaviour : Gamemode
    {
        private List<PlayerBehaviour> _players = new List<PlayerBehaviour>();
        private List<PlayerBehaviour> finished_players = new List<PlayerBehaviour>();
        public Transform Finish;
        
        public Text LiveRaceText;
        public Text EndOfRaceText;
        public Text PreRaceTimerText;

        public GameObject PreRaceTimerUI;
        public GameObject EndOfRaceUI;

        [SyncVar] private string _t;
        [SyncVar] private string _endofrace;
        [SyncVar(hook = "OnPreRaceTimerChange")] public float PreRaceTimer;
        [SyncVar(hook = "OnPostRaceTimerChange")] public float PostRaceTimer;
        [SyncVar(hook = "OnRaceTimerChange")] public float RaceTimer;

        private void OnPreRaceTimerChange(float timerChange)
        {
            PreRaceTimer = timerChange;
            PreRaceTimer = Mathf.Clamp(PreRaceTimer, 0.0f, PreMatchTimer);
        }
        private void OnPostRaceTimerChange(float timerChange)
        {
            PostRaceTimer = timerChange;
            PostRaceTimer = Mathf.Clamp(PostRaceTimer, 0.0f, 999999);
        }
        private void OnRaceTimerChange(float timerChange)
        {
            RaceTimer = timerChange;
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

        public void RestartRace(string scene)
        {
            var players = FindObjectsOfType<PlayerBehaviour>().ToList();
            players.ForEach(player =>
            {
                var player_networkID = player.GetComponent<NetworkIdentity>();
                netCompanion.ServerRespawnPlayer(player_networkID);
            });

            netCompanion.Server_ChangeScene(scene);
        }

        private void EndOfRace()
        {
            PostRaceTimer -= Time.deltaTime;
            if (PostRaceTimer <= 0.0f)
            {
                Players.ForEach(player =>
                {
                    var client_scene = SceneManager.GetActiveScene();
                    var network_identity = player.GetComponent<NetworkIdentity>();
                    netCompanion.RpcServer_Disconnect(network_identity, client_scene.name);
                });

                //CHECK IF THERE ARE NO PLAYERS CONNECTED BEFORE CHANGING THE SCENE
                //server.Server_LANDisconnectAll();

                if(Players.Count == 0)
                {
                    var server_scene = SceneManager.GetActiveScene();
                    EndMatchLAN(server_scene.name);
                }
            }

            _endofrace = "RACE COMPLETE \n";
            //EndOfRaceText.text += "TIME REMAINING: " + RaceTimer.ToString() + "\n";
            _endofrace += "NEW RACE IN: " + PostRaceTimer.ToString() + "\n";

            //LIST ALL PLAYERS SORTED BY DISTANCE TO GOAL
            int place = 1;
            Players.ForEach(player =>
            {
                _endofrace += string.Format("{0}.{1}", place, player.playerName);
                place += 1;
            });

            #region OLD
            //var _i = 1;
            //for (int i = finished_players.Count; i > 0; i--)
            //{
            //    EndOfRaceText.text += _i.ToString() + ". " + finished_players[i - 1].playerName + "\n";
            //    _i++;
            //} 
            #endregion
        }

        //[ClientRpc] private void RpcPlayerEndOfRace(NetworkIdentity player)
        //{
        //    if (player.isLocalPlayer)
        //    {
        //        PlayerResultsPanel.SetActive(true);

        //        PlayerEndOfRaceText.text = "RACE COMPLETE \n";
        //        PlayerEndOfRaceText.text += "FINISH TIME: " + RaceTimer.ToString() + "\n";

        //        var _i = 1;
        //        for (int i = finished_players.Count; i > 0; i--)
        //        {
        //            EndOfRaceText.text += _i.ToString() + ". " + finished_players[i - 1].playerName + "\n";
        //            _i++;
        //        }
        //    }
        //}

        //private void Start()
        //{
        //    if (isServer)
        //    {
        //        //players = FindObjectsOfType<PlayerBehaviour>().ToList();
        //        //net.SpawnAllVehicles();
        //        //timer = EndOfRaceTimer;
        //    }
        //}

        //private void FixedUpdate()
        //{
        //    if (isServer)
        //    {
        //        //CHECK TO SEE IF THE LIST OF PLAYERBEHAVIOUR IS EMPTY, FIND ALL OF THEM BY TYPE 'PLAYERBEHAVIOUR'
        //        //if (players.Count <= 0)
        //        //{
        //        //    players = FindObjectsOfType<PlayerBehaviour>().ToList();
        //        //}

        //        //if (finished_players.Count == players.Count)
        //        //{
        //        //    isRaceFinished = true;
        //        //    return;
        //        //}
        //        //else
        //        //{
        //        //    isRaceFinished = false;
        //        //}


        //    }
        //}

        private void LateUpdate()
        {


            //SYNC UI TEXT

            LiveRaceText.text = "";
            LiveRaceText.text = _t;

            PreRaceTimerText.text = "";
            PreRaceTimerText.text = "BEGIN IN: " + PreRaceTimer.ToString();

            EndOfRaceText.text = "";
            EndOfRaceText.text = _endofrace;

            #region OLD
            //if (isServer)
            //{
            //    if (isRaceFinished)
            //    {
            //        timer -= Time.deltaTime;
            //        if (timer <= 0.0f)
            //        {
            //            RestartRace("99.debug");
            //        }
            //    }
            //}

            //if (isRaceFinished)
            //{
            //    EndOfRace();
            //} 
            #endregion
        }

        //NEEDS WORK
        //INVOKE A CMD CALL TO DISABLE THE PLAYER MODEL ON THE SERVER/CONNECTED CLIENTS
        //ADD A BOOLEAN VARIABLE TO DETERMINE IF THE PLAYER IS 'ACTIVE' OR NOT
        //WHEN THE PLAYER COLLIDES WITH THE 'FINISH LINE' DISABLE INPUT CONTROLLER, PLAYER MODEL
        //ENABLE THE END OF RACE CANVAS FOR THIS PLAYER
        private void OnTriggerStay(Collider other)
        {
            if (!isServer)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                var p_behaviour = other.GetComponentInParent<PlayerBehaviour>();
                if (p_behaviour.isActive)
                {
                    Debug.Log("COLLISION WITH PLAYER");
                    var p_netIdentity = other.GetComponentInParent<NetworkIdentity>();

                    Debug.Log(p_behaviour.gameObject.name + " FINISHED");
                    netCompanion.PlayerFinishRace(p_netIdentity, " FINISHED RACE! \n");
                    finished_players.Add(p_behaviour);

                    var ic = p_behaviour.ic;
                    ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
                    p_behaviour.isActive = false;
                    p_behaviour.RpcSetActive(false);

                    var sorted_players = Players.OrderBy(x => Vector3.Distance(x.transform.position, Finish.transform.position)).ToList();
                    Players = sorted_players;
                    WinCondition = true;
                    //RpcPlayerEndOfRace(p_netIdentity);
                }
            }

            if (other.CompareTag("Vehicle"))
            {
                var vehicle_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                if (vehicle_behaviour.seatedPlayer != null)
                {
                    vehicle_behaviour.seatedPlayer.RemovePlayerFromVehicle();
                }

                netCompanion.Server_Destroy(vehicle_behaviour.gameObject);

                #region OLD
                //Debug.Log("COLLISION WITH VEHICLE");
                //var v_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                //players.ForEach(player =>
                //{
                //    if (player.vehicle == v_behaviour)
                //    {
                //        Debug.Log(player.gameObject.name + " FINISHED");

                //        finished_players.Add(player);

                //        var ic = player.ic;
                //        ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
                //        player.model.SetActive(false);
                //        //player.enabled = false;
                //        Destroy(v_behaviour.gameObject);
                //    }
                //}); 
                #endregion
            }
        }

        public override bool CheckWinCondition()
        {
            return false;
        }

        public override bool CheckLoseCondition()
        {
            if (RaceTimer <= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnWinCondition()
        {
            EndOfRace();
            TogglePlayerControl(false);
            ToggleEndOfRaceUI(true);
            RpcToggleEndOfRaceUI(true);
        }

        public override void OnLoseCondition()
        {
            EndOfRace();
            TogglePlayerControl(false);
            ToggleEndOfRaceUI(true);
            RpcToggleEndOfRaceUI(true);
        }

        public override void GameLoop()
        {
            PreRaceTimer -= Time.deltaTime;

            if(Players.Count <= 0)
            {
                RestartPreMatchTimer();
            }

            if(PreRaceTimer > 0.0f)
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

                RaceTimer -= Time.deltaTime; //DECREMENT THE RACE TIMER
            }

            //SORT THE LIST OF PLAYERS BY THEIR DISTANCE TO THE 'FINISH'
            var sorted_players = Players.OrderBy(x => Vector3.Distance(x.transform.position, Finish.transform.position)).ToList();
            _players = sorted_players;

            _t = "";
            _t += "Timer: " + RaceTimer.ToString() + "\n";

            for (int i = 0; i < _players.Count; i++)
            {
                _t += (i + 1).ToString() + ". " + _players[i].playerName + "\n";
            }
        }

        public override void Initialize()
        {
            RaceTimer = MatchTimer;
            PreRaceTimer = PreMatchTimer;
            PostRaceTimer = PostMatchTimer;
            _players = Players;
        }

        public override void RestartPreMatchTimer()
        {
            if(!MatchBegun)
            {
                PreRaceTimer = PreMatchTimer;
            }
        }
    }
}