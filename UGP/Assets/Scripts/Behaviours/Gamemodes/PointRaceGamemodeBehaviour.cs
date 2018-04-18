using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UGP
{
    public class PointRaceGamemodeBehaviour : NetworkBehaviour
    {
        public List<PlayerBehaviour> players = new List<PlayerBehaviour>();
        public List<PlayerBehaviour> finished_players = new List<PlayerBehaviour>();
        public Transform Finish;

        public InGameNetworkBehaviour server;
        public Text LiveRaceText;
        public Text EndOfRaceText;
        public Text PlayerEndOfRaceText;

        public GameObject ResultsPanel;
        public GameObject PlayerResultsPanel;

        [SyncVar] private string _t;
        [SyncVar(hook = "OnRaceTimerChange")] public float RaceTimer;
        [SyncVar(hook = "OnEndOfRaceTimerChange")] public float EndOfRaceTimer;
        [SyncVar(hook = "OnTimerChange")] private float timer = 0.0f;
        [SyncVar(hook = "OnRaceFinishChange")] public bool isRaceFinished;

        private void OnRaceTimerChange(float timerChange)
        {
            RaceTimer = timerChange;
        }
        private void OnEndOfRaceTimerChange(float timerChange)
        {
            EndOfRaceTimer = timerChange;
        }
        private void OnTimerChange(float timerChange)
        {
            timer = timerChange;
        }
        private void OnRaceFinishChange(bool finished)
        {
            isRaceFinished = finished;
        }

        public void RestartRace(string scene)
        {
            var players = FindObjectsOfType<PlayerBehaviour>().ToList();
            players.ForEach(player =>
            {
                var player_networkID = player.GetComponent<NetworkIdentity>();
                server.ServerRespawnPlayer(player_networkID);
            });

            server.Server_ChangeScene(scene);
        }

        private void EndOfRace()
        {
            PlayerResultsPanel.SetActive(false);
            ResultsPanel.SetActive(true);

            EndOfRaceText.text = "RACE COMPLETE \n";
            //EndOfRaceText.text += "TIME REMAINING: " + RaceTimer.ToString() + "\n";
            EndOfRaceText.text += "RACE RESTART IN: " + timer.ToString() + "\n";
            var _i = 1;
            for (int i = finished_players.Count; i > 0; i--)
            {
                EndOfRaceText.text += _i.ToString() + ". " + finished_players[i - 1].playerName + "\n";
                _i++;
            }
        }

        [ClientRpc] private void RpcPlayerEndOfRace(NetworkIdentity player)
        {
            if (player.isLocalPlayer)
            {
                PlayerResultsPanel.SetActive(true);

                PlayerEndOfRaceText.text = "RACE COMPLETE \n";
                PlayerEndOfRaceText.text += "FINISH TIME: " + RaceTimer.ToString() + "\n";

                var _i = 1;
                for (int i = finished_players.Count; i > 0; i--)
                {
                    EndOfRaceText.text += _i.ToString() + ". " + finished_players[i - 1].playerName + "\n";
                    _i++;
                }
            }
        }

        private void Start()
        {
            if (isServer)
            {
                players = FindObjectsOfType<PlayerBehaviour>().ToList();
                //net.SpawnAllVehicles();
                timer = EndOfRaceTimer;
            }
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                //CHECK TO SEE IF THE LIST OF PLAYERBEHAVIOUR IS EMPTY, FIND ALL OF THEM BY TYPE 'PLAYERBEHAVIOUR'
                if (players.Count <= 0)
                {
                    players = FindObjectsOfType<PlayerBehaviour>().ToList();
                }

                //if (finished_players.Count == players.Count)
                //{
                //    isRaceFinished = true;
                //    return;
                //}
                //else
                //{
                //    isRaceFinished = false;
                //}

                RaceTimer -= Time.deltaTime;
                if (RaceTimer <= 0.0f)
                {
                    isRaceFinished = true;
                    return;
                }

                //SORT THE LIST OF PLAYERS BY THEIR DISTANCE TO THE 'FINISH'
                var sorted_players = players.OrderBy(x => Vector3.Distance(x.transform.position, Finish.transform.position)).ToList();
                players = sorted_players;

                _t = "";
                _t += "Timer: " + RaceTimer.ToString() + "\n";

                for (int i = 0; i < players.Count; i++)
                {
                    _t += (i + 1).ToString() + ". " + players[i].playerName + "\n";
                }
            }

            LiveRaceText.text = "";
            LiveRaceText.text = _t;
        }

        private void LateUpdate()
        {
            if (isServer)
            {
                if (isRaceFinished)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0.0f)
                    {
                        RestartRace("99.debug");
                    }
                }
            }

            if (isRaceFinished)
            {
                EndOfRace();
            }
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
                    server.PlayerFinishRace(p_netIdentity, " FINISHED RACE! \n");
                    finished_players.Add(p_behaviour);

                    var ic = p_behaviour.ic;
                    ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
                    p_behaviour.isActive = false;
                    p_behaviour.RpcSetActive(false);
                    RpcPlayerEndOfRace(p_netIdentity);
                }
            }

            if (other.CompareTag("Vehicle"))
            {
                var vehicle_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                if (vehicle_behaviour.seatedPlayer != null)
                {
                    vehicle_behaviour.seatedPlayer.RemovePlayerFromVehicle();
                }

                server.Server_Destroy(vehicle_behaviour.gameObject);

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
    }
}