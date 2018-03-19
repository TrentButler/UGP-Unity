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

        public InGameNetworkBehaviour net;
        public Text LiveRaceText;
        public Text EndOfRaceText;
        [SyncVar] private string _t;
        [SyncVar] public float RaceTimer;

        public GameObject ResultsPanel;
        [SyncVar] public float EndOfRaceTimer;
        [SyncVar] private float timer = 0.0f;

        [ClientRpc]
        public void RpcRestartRace()
        {
            SceneManager.LoadScene("03.Race");
        }

        private void EndOfRace()
        {
            ResultsPanel.SetActive(true);

            EndOfRaceText.text = "RACE COMPLETE \n";
            EndOfRaceText.text += "TIME REMAINING: " + RaceTimer.ToString() + "\n";
            EndOfRaceText.text += "RACE RESTART IN: " + timer.ToString();
            var _i = 1;
            for (int i = finished_players.Count; i > 0; i--)
            {
                EndOfRaceText.text += _i.ToString() + ". " + finished_players[i].name + "\n";
                _i++;
            }

            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                RpcRestartRace();
            }
        }

        private void Start()
        {
            if (isServer)
            {
                players = FindObjectsOfType<PlayerBehaviour>().ToList();
                net.SpawnAllVehicles();
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
                }

                //SORT THE LIST OF PLAYERS BY THEIR DISTANCE TO THE 'FINISH'
                var sorted_players = players.OrderBy(x => Vector3.Distance(x.transform.position, Finish.transform.position)).ToList();
                players = sorted_players;

                _t = "";
                _t += "Timer: " + RaceTimer.ToString() + "\n";

                for (int i = 0; i < players.Count; i++)
                {
                    _t += (i + 1).ToString() + ". " + players[i].name + "\n";
                }
            }

            LiveRaceText.text = "";
            LiveRaceText.text = _t;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var p_behaviour = other.GetComponent<PlayerBehaviour>();
                var p_netIdentity = other.GetComponent<NetworkIdentity>();

                finished_players.Add(p_behaviour);

                p_behaviour.ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
            }

            if (other.CompareTag("Vehicle"))
            {
                var v_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                players.ForEach(player =>
                {
                    if (player.vehicle == v_behaviour)
                    {
                        finished_players.Add(player);
                        player.ic.enabled = false;
                        v_behaviour.ic.enabled = false;
                    }
                });
            }
        }
    }
}