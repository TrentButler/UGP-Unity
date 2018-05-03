using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace UGP
{

    public class IntPlayerName : NetworkLobbyPlayer
    {


        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        public InputField nameInput;

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }
        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }



    }
}