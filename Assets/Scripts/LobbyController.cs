using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Element

    public TMP_Text lobbyNameText;

    //Player Data
    public GameObject playerListViewContent;
    public GameObject playerListItemPrefab;
    public GameObject localPlayerObejct;

    //other Data
    public ulong currentLobbyID;
    public bool playerItemCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController localpalyerController;

    public Button startGameButton;


    //Manager
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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void CheckIfHostReady()
    {
        //Debug.Log(localpalyerController.playerIdNumber);
        //if (localpalyerController.playerIdNumber == 1)
        {
            startGameButton.interactable = true;
        }
        //else
        {
            //startGameButton.interactable = false;
        }
    }
    public void UpdateLobbyName()
    {
        currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!playerItemCreated)
            CreateHostPlayerItem();

        if (playerListItems.Count < Manager.gamePlayers.Count)
            CreateCliebtPlayerItem();
        if (playerListItems.Count > Manager.gamePlayers.Count)
            RemovePlayerItem();
        if (playerListItems.Count == Manager.gamePlayers.Count)
            UpdatePlayerItem();
    }
    public void FindLocalPlayer()
    {
        localPlayerObejct = GameObject.Find("LocalGamePlayer");
        localpalyerController = localPlayerObejct.GetComponent<PlayerObjectController>();

    }

    public void CreateHostPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.gamePlayers)
        {
            GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
            PlayerListItem newPlayerListScripts = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerListScripts.playerName = player.playerName;
            newPlayerListScripts.connectionID = player.connectionID;
            newPlayerListScripts.playerSteamID = player.playerSteamID;
            newPlayerListScripts.SetPlayerValues();

            newPlayerItem.transform.SetParent(playerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;


            playerListItems.Add(newPlayerListScripts);
        }
        playerItemCreated = true;
    }
    public void CreateCliebtPlayerItem()
    {
        foreach(PlayerObjectController player in Manager.gamePlayers)
        {
            if (!playerListItems.Any(b => b.connectionID == player.connectionID))
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerListScripts = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerListScripts.playerName = player.playerName;
                newPlayerListScripts.connectionID = player.connectionID;
                newPlayerListScripts.playerSteamID = player.playerSteamID;
                newPlayerListScripts.SetPlayerValues();

                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;
                playerListItems.Add(newPlayerListScripts);
            }
        }
    }
    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.gamePlayers)
        {
            foreach (PlayerListItem playerListItemScript in playerListItems)
            {
                if (playerListItemScript.connectionID == player.connectionID)
                {
                    playerListItemScript.playerName = player.playerName;
                    playerListItemScript.SetPlayerValues();
                }
            }
        }
        CheckIfHostReady();
    }
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem playerListItem in playerListItems)
        {
            if (!Manager.gamePlayers.Any(b => b.connectionID == playerListItem.connectionID))
                playerListItemToRemove.Add(playerListItem);
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem _playerListItemToRemove in playerListItemToRemove)
            {
                GameObject objectToRemove = _playerListItemToRemove.gameObject;
                playerListItems.Remove(_playerListItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
    public void StartGame(string sceneName)
    {
        localpalyerController.CanStartGame(sceneName);
    }


}
