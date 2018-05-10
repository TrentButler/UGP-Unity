using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class OfflineVehicleAudioBehaviour : MonoBehaviour
    {
        public AudioClip EngineSound;
        public AudioSource EngineAudioSource;
        public float MinThrottlePitch = 0.4f;
        public float MaxThrottlePitch = 2f;

        public float MinEngineSoundDistance = 50f;
        public float MaxEngineSoundDistance = 1000f;
        public float EngineDopplerLevel = 1f;
        [Range(0f, 1f)] public float EngineMasterVolume = 0.5f;

        public OfflineVehicleController vehicleController;

        private void Awake()
        {
            if(vehicleController == null)
            {
                vehicleController = GetComponent<OfflineVehicleController>();
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

        private void Update()
        {
            var enginePowerProportion = Mathf.InverseLerp(0, vehicleController.MaxSpeed, vehicleController.currentVehiclePower);
         
            EngineAudioSource.pitch = Mathf.Lerp(MinThrottlePitch, MaxThrottlePitch, enginePowerProportion);
            
            EngineAudioSource.volume = Mathf.InverseLerp(0, vehicleController.MaxSpeed * EngineMasterVolume, vehicleController.currentVehiclePower);
        }
    }

}