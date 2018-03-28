﻿using System.Linq;
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
        [SyncVar(hook = "OnTextChange")] public string _t;
        [SyncVar] public float RaceTimer;

        public GameObject ResultsPanel;
        [SyncVar] public float EndOfRaceTimer;
        [SyncVar] private float timer = 0.0f;

        [ClientRpc]
        public void RpcRestartRace()
        {
            NetworkServer.Reset();
            SceneManager.LoadScene("69.OfflineScene");
        }

        public void OnTextChange(string change)
        {
            _t = change;
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
                net.SpawnAllVehiclesWithAllItems();
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
                    _t += (i + 1).ToString() + ". " + players[i].name + "\n";
                }

                finished_players.ForEach(player =>
                {
                    if(player.isLocalPlayer)
                    {
                        player.model.SetActive(false);
                        player.ic.enabled = false;
                        ResultsPanel.SetActive(true);
                    }
                });
            }

            LiveRaceText.text = "";
            LiveRaceText.text = _t;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("COLLISION WITH PLAYER");

                var p_behaviour = other.GetComponent<PlayerBehaviour>();
                var p_netIdentity = other.GetComponent<NetworkIdentity>();

                Debug.Log(p_behaviour.gameObject.name + " FINISHED");
                finished_players.Add(p_behaviour);

                var ic = p_behaviour.ic;
                ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
                p_behaviour.model.SetActive(false);
                //p_behaviour.enabled = false;
            }

            if (other.CompareTag("Vehicle"))
            {
                Debug.Log("COLLISION WITH VEHICLE");
                var v_behaviour = other.GetComponentInParent<VehicleBehaviour>();
                players.ForEach(player =>
                {
                    if (player.vehicle == v_behaviour)
                    {
                        Debug.Log(player.gameObject.name + " FINISHED");

                        finished_players.Add(player);

                        var ic = player.ic;
                        ic.enabled = false; //DISABLE THE PLAYER MOVEMENT
                        player.model.SetActive(false);
                        //player.enabled = false;
                        Destroy(v_behaviour.gameObject);
                    }
                });
            }
        }
    }
}