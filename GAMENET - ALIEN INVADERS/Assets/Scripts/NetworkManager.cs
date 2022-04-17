using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using UnityEngine.Assertions;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public TMP_InputField PlayerNameInput;

    [Header("Connecting UI")]
    public GameObject ConnectingUIPanel;

    [Header("GameOptions UI")]
    public GameObject GameOptionsPanel;

    [Header("CreateRoom UI")]
    public GameObject CreateRoomPanel;
    public GameObject CreatingRoomPanel;
    public TMP_InputField RoomNameInput;
    public TMP_InputField PlayerCountInput;

    [Header("Inside Room UI")]
    public GameObject InsideRoomPanel;
    public TextMeshProUGUI RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;

    private Dictionary<int, GameObject> playerListGameObjects;



    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Public Functions
    public void ActivatePanel(string panelToActivate)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelToActivate));
        ConnectingUIPanel.SetActive(ConnectingUIPanel.name.Equals(panelToActivate));
        GameOptionsPanel.SetActive(GameOptionsPanel.name.Equals(panelToActivate));
        CreateRoomPanel.SetActive(CreateRoomPanel.name.Equals(panelToActivate));
        CreatingRoomPanel.SetActive(CreatingRoomPanel.name.Equals(panelToActivate));
        InsideRoomPanel.SetActive(InsideRoomPanel.name.Equals(panelToActivate));    
    }
    #endregion

    #region Private Functions

    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        return true;
    }


    #endregion

    #region Photon Callbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has Connected to Photon");
        ActivatePanel(GameOptionsPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created");
    }
    public override void OnJoinedRoom()
    {
        ActivatePanel(InsideRoomPanel.name);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            Assert.IsNotNull(PlayerListPrefab, "Player List Prefab is not set");
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            playerListItem.transform.SetParent(PlayerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;
            playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;
            if(player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool)isPlayerReady);
            }
            playerListGameObjects.Add(player.ActorNumber, playerListItem);
            StartGameButton.SetActive(false);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        playerListItem.transform.SetParent(PlayerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;
        playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        
        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject; 
        if(playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if(changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool)isPlayerReady);

            }
        }
        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient) // called when the original master client left the room and a new master client has been chosen 
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsPanel.name);
        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // called when a player leaves the room
    {
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    #endregion


    #region UI Callbacks

    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingUIPanel.name);
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Invalid Name");
        }
    }

    public void OnCreateGameButtonClicked()
    {
        ActivatePanel(CreatingRoomPanel.name);
        string roomName = RoomNameInput.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(100, 5000);
        }
        RoomOptions roomOptions = new RoomOptions();
        int roomPlayerCount = int.Parse(PlayerCountInput.text);
        if(roomPlayerCount > 2 || roomPlayerCount < 0)
        {
            Debug.Log("Max players allowed are 2");
            roomOptions.MaxPlayers = 2;
        }
        else
        {
            roomOptions.MaxPlayers = (byte)roomPlayerCount;
        }
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    

    #endregion
}
