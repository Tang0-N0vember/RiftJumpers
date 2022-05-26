using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data

    [SyncVar] public int connectionID;
    [SyncVar] public int playerIdNumber;
    [SyncVar] public ulong playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }
    public override void OnStartClient()
    {
        Manager.gamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }
    public override void OnStopClient()
    {
        Manager.gamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
    [Command]
    private void CmdSetPlayerName(string playername)
    {
        this.PlayerNameUpdate(this.playerName, playername);
    }
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)//Host
            this.playerName = newValue;

        if (isClient)//Client
            LobbyController.Instance.UpdatePlayerList();

    }
    public void CanStartGame(string sceneName)
    {
        if (hasAuthority)
            CmdCanStartGame(sceneName);
    }
    [Command]
    public void CmdCanStartGame(string sceneName)
    {
        manager.StartGame(sceneName);
    }


}
