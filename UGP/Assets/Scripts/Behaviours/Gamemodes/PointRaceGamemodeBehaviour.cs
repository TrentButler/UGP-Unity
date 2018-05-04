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

        public Transform Storm;
        [Range(0.0001f, 999999)] public float StormTravelSpeed;
        [SyncVar(hook = "OnStormPositionChange")] public Vector3 StormPosition;
        [SyncVar(hook = "OnStormRotationChange")] public Quaternion StormRotation;
        [SyncVar(hook = "OnStormProgressionChange")] public float StormProgression;
        [SyncVar(hook = "OnTotalStormDistanceChange")] public float TotalStormDistance;
        public float currentStormDistFromFinish;
        public Slider StormProgressionSlider;

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
        [SyncVar(hook = "OnPreStormTimerChange")] [Range(0.001f, 999999)] public float PreStormTimer;

        private void OnPreRaceTimerChange(float timerChange)
        {
            PreRaceTimer = timerChange;
            PreRaceTimer = Mathf.Clamp(PreRaceTimer, 0.0f, PreMatchTimer);
        }
        private void OnPostRaceTimerChange(float timerChange)
        {
            PostRaceTimer = timerChange;
            PostRaceTimer = Mathf.Clamp(PostRaceTimer, 0.0f, PostMatchTimer);
        }
        private void OnRaceTimerChange(float timerChange)
        {
            RaceTimer = timerChange;
            RaceTimer = Mathf.Clamp(RaceTimer, 0, MatchTimer);
        }
        private void OnPreStormTimerChange(float timerChange)
        {
            PreStormTimer = timerChange;
            PreStormTimer = Mathf.Clamp(PreStormTimer, 0, 999999);
        }
        private void OnStormPositionChange(Vector3 positionChange)
        {
            StormPosition = positionChange;
        }
        private void OnStormRotationChange(Quaternion rotationChange)
        {
            StormRotation = rotationChange;
        }
        private void OnStormProgressionChange(float progressionChange)
        {
            StormProgression = progressionChange;
        }
        private void OnTotalStormDistanceChange(float totalDistanceChange)
        {
            TotalStormDistance = totalDistanceChange;
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

        private float GetStormProgression()
        {
            var currentDistance = Vector3.Distance(StormPosition, Finish.position);
            //currentStormDistFromFinish = currentDistance;
            //var progression_displacement = currentDistance / TotalStormDistance;
            //var calc = new Vector3(progression_displacement, 0, 0);
            //return calc.normalized.x;

            return -(1 / TotalStormDistance) * currentDistance + 1;
        }

        private void Start()
        {
            if(!isServer)
            {
                return;
            }

            StormPosition = Storm.position;
            StormRotation = Storm.rotation;
            TotalStormDistance = Vector3.Distance(StormPosition, Finish.position);
        }

        private void LateUpdate()
        {
            var StormGOActive = Storm.gameObject.activeInHierarchy;
            if (!StormGOActive)
            {
                Storm.gameObject.SetActive(true);
            }

            //SYNC STORM POSITION/ROTATION
            Storm.transform.position = StormPosition;
            Storm.transform.rotation = StormRotation;

            //SYNC UI STUFF

            LiveRaceText.text = "";
            LiveRaceText.text = _t;

            PreRaceTimerText.text = "";
            PreRaceTimerText.text = "BEGIN IN: " + PreRaceTimer.ToString();

            EndOfRaceText.text = "";
            EndOfRaceText.text = _endofrace;

            StormProgressionSlider.value = StormProgression;

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
                PreStormTimer -= Time.deltaTime;
                if(PreStormTimer <= 0.0f)
                {
                    var StormGOActive = Storm.gameObject.activeInHierarchy;
                    if(!StormGOActive)
                    {
                        Storm.gameObject.SetActive(true);
                    }

                    //MOVE AND ROTATE THE STORM ON THE SERVER
                    StormPosition = Vector3.Lerp(Storm.position, Finish.position, Time.deltaTime * StormTravelSpeed);
                    Storm.transform.LookAt(Finish);
                    StormRotation = Storm.transform.rotation;

                    StormProgression = GetStormProgression();
                }
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