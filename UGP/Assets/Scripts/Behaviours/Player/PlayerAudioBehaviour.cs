using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class PlayerAudioBehaviour : NetworkBehaviour
    {
        public AudioClip OnHitSound;
        public AudioClip WalkSound;
        public AudioClip OnDeathSound;
        public AudioSource OnHitAudioSource;
        public AudioSource WalkAudioSource;

        public NetworkUserControl userControl;

        [SyncVar(hook = "OnCurrentVerticalInputChange")] public float current_vertical_input = 0.0f;
        [SyncVar(hook = "OnCurrentHorizontalInputChange")] public float current_horizontal_input = 0.0f;
        private void OnCurrentVerticalInputChange(float inputChange)
        {
            current_vertical_input = inputChange;
        }
        private void OnCurrentHorizontalInputChange(float inputChange)
        {
            current_horizontal_input = inputChange;
        }

        [Command] public void CmdCurrentVerticalInput(float currentInput)
        {
            current_vertical_input = currentInput;
        }
        [Command] public void CmdCurrentHorizontalInput(float currentInput)
        {
            current_horizontal_input = currentInput;
        }

        [Command] public void CmdPlayerHit()
        {
            RpcPlayerHit();
        }
        [ClientRpc] public void RpcPlayerHit()
        {
            PlayPlayerHitSound();
        }

        [Command] public void CmdPlayerDead()
        {
            RpcPlayerDead();
        }
        [ClientRpc] public void RpcPlayerDead()
        {
            PlayPlayerDeadSound();
        }

        //NEEDS WORK
        [Command] public void CmdPlayerWalk()
        {
            RpcPlayerWalk();
        }
        [ClientRpc] public void RpcPlayerWalk()
        {
            var move_vector = new Vector3(current_horizontal_input, 0, current_vertical_input);
            if(move_vector.magnitude > 0.0f)
            {
                PlayPlayerWalkSound();
            }
            else
            {
                StopPlayerWalkSound();
            }
        }

        public void PlayPlayerHitSound()
        {
            OnHitAudioSource.Stop();
            OnHitAudioSource.clip = OnHitSound;
            OnHitAudioSource.Play();
        }
        public void PlayPlayerDeadSound()
        {
            OnHitAudioSource.Stop();
            OnHitAudioSource.clip = OnDeathSound;
            OnHitAudioSource.Play();
        }
        public void PlayPlayerWalkSound()
        {
            //WalkAudioSource.Stop();
            //WalkAudioSource.Play();
        }
        public void StopPlayerWalkSound()
        {
            //WalkAudioSource.Stop();
        }

        private void Start()
        {
            OnHitAudioSource.clip = OnHitSound;
            WalkAudioSource.clip = WalkSound;
        }

        private void FixedUpdate()
        {
            if(!isLocalPlayer)
            {
                return;
            }

            current_horizontal_input = userControl.GetCurrentHorizontalInput();
            current_vertical_input = userControl.GetCurrentVerticalInput();
            CmdCurrentHorizontalInput(current_horizontal_input);
            CmdCurrentVerticalInput(current_vertical_input);
        }
    }
}