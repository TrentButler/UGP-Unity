using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class VehicleAudioBehaviour : NetworkBehaviour
    {
        public AudioClip EngineSound;
        public AudioSource EngineAudioSource;
        public float MinThrottlePitch = 0.4f;
        public float MaxThrottlePitch = 2f;
        public float ForwardSpeedMultiplier = 0.001f;

        public float MinEngineSoundDistance = 50f;
        public float MaxEngineSoundDistance = 1000f;
        public float EngineDopplerLevel = 1f;
        [Range(0f, 1f)] public float EngineMasterVolume = 0.5f;

        public DefaultVehicleController vehicleController;

        [SyncVar(hook = "OnCurrentVerticalThrottleChange")] public float current_vertical_throttle = 0.0f;
        [SyncVar(hook = "OnCurrentHorizontalThrottleChange")] public float current_horizontal_throttle = 0.0f;
        [SyncVar(hook = "OnVehicleMaxSpeedChange")] public float vehicle_max_speed;
        private void OnCurrentVerticalThrottleChange(float throttleChange)
        {
            current_vertical_throttle = throttleChange;
        }
        private void OnCurrentHorizontalThrottleChange(float throttleChange)
        {
            current_horizontal_throttle = throttleChange;
        }
        private void OnVehicleMaxSpeedChange(float speedChange)
        {
            vehicle_max_speed = speedChange;
        }

        [Command] public void CmdCurrentVerticalThrottle(float currentThrottle)
        {
            current_vertical_throttle = currentThrottle;
        }
        [Command] public void CmdCurrentHorizontalThrottle(float currentThrottle)
        {
            current_horizontal_throttle = currentThrottle;
        }
        [Command] public void CmdVehicleMaxSpeed(float maxSpeed)
        {
            vehicle_max_speed = maxSpeed;
        }

        private void Awake()
        {
            if (vehicleController == null)
            {
                vehicleController = GetComponent<DefaultVehicleController>();
            }

            EngineAudioSource.playOnAwake = false;
            EngineAudioSource.clip = EngineSound;

            EngineAudioSource.minDistance = MinEngineSoundDistance;
            EngineAudioSource.maxDistance = MaxEngineSoundDistance;
            EngineAudioSource.loop = true;
            EngineAudioSource.dopplerLevel = EngineDopplerLevel;

            Update();

            EngineAudioSource.Play();
        }

        private void FixedUpdate()
        {
            if (hasAuthority && !isServer)
            {
                current_vertical_throttle = vehicleController.currentVehiclePower;
                current_horizontal_throttle = Mathf.Abs(vehicleController.currentVehicleStrafe);
                vehicle_max_speed = vehicleController.MaxSpeed;
                CmdCurrentVerticalThrottle(vehicleController.currentVehiclePower);
                CmdCurrentHorizontalThrottle(Mathf.Abs(vehicleController.currentVehicleStrafe));
                CmdVehicleMaxSpeed(vehicleController.MaxSpeed);
            }
        }

        private void Update()
        {
            var enginePowerProportion = Mathf.InverseLerp(0, vehicle_max_speed, current_vertical_throttle);

            EngineAudioSource.pitch = Mathf.Lerp(MinThrottlePitch, MaxThrottlePitch, enginePowerProportion);

            EngineAudioSource.volume = Mathf.InverseLerp(0, vehicle_max_speed * EngineMasterVolume, current_vertical_throttle);
        }
    }

}