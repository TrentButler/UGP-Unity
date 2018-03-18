using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetPlayerLobbyInfo : Prototype.NetworkLobby.LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        var lb = lobbyPlayer.GetComponent<Prototype.NetworkLobby.LobbyPlayer>();
        gamePlayer.name = lb.playerName;

        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
    }
}
