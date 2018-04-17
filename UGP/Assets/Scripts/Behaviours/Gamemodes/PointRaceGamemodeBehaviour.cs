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
        private List<PlayerBehaviour> finished_players = new List<PlayerBehaviour>();
        public Transform Finish;

        public InGameNetworkBehaviour server;
        public Text LiveRaceText;
        public Text EndOfRaceText;
        [SyncVar] private string _t;
        [SyncVar] public float RaceTimer;

        public GameObject ResultsPanel;
        [SyncVar] public float EndOfRaceTimer;
        [SyncVar] private float timer = 0.0f;

        [ClientRpc] public void RpcRestartRace(string scene)
        {
            NetworkManager.singleton.ServerChangeScene(scene);
            //NetworkServer.Reset();
            //SceneManager.LoadScene("03.Race");
        }

        private void EndOfRace()
        {
            ResultsPanel.SetActive(true);

            EndOfRaceText.text = "RACE COMPLETE \n";
            EndOfRaceText.text += "TIME REMAINING: " + RaceTimer.ToString() + "\n";
            EndOfRaceText.text += "RACE RESTART IN: " + timer.ToString() + "\n";
            var _i = 1;
            for (int i = finished_players.Count; i > 0; i--)
            {
                EndOfRaceText.text += _i.ToString() + ". " + finished_players[i - 1].playerName + "\n";
                _i++;
            }

            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                RpcRestartRace("99.debug");
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

                if (finished_players.Count == players.Count)
                {
                    EndOfRace();
                    return;
                }
                else
                {
                    ResultsPanel.SetActive(false);
                }

                RaceTimer -= Time.deltaTime;
                if (RaceTimer <= 0.0f)
                {
                    EndOfRace();
                    return;
                }
                else
                {
                    ResultsPanel.SetActive(false);
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
            finished_players.ForEach(player =>
            {
                if (player.isLocalPlayer)
                {
                    ResultsPanel.SetActive(true);
                }
            });
        }

        //NEEDS WORK
        //INVOKE A CMD CALL TO DISABLE THE PLAYER MODEL ON THE SERVER/CONNECTED CLIENTS
        //ADD A BOOLEAN VARIABLE TO DETERMINE IF THE PLAYER IS 'ACTIVE' OR NOT
        //WHEN THE PLAYER COLLIDES WITH THE 'FINISH LINE' DISABLE INPUT CONTROLLER, PLAYER MODEL
        //ENABLE THE END OF RACE CANVAS FOR THIS PLAYER
        private void OnTriggerStay(Collider other)
        {
            if(!isServer)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                var p_behaviour = other.GetComponentInParent<PlayerBehaviour>();
                if(p_behaviour.isActive)
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
                }
            }

            if (other.CompareTag("Vehicle"))
            {
                var vehicle_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                if(vehicle_behaviour.seatedPlayer != null)
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