using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Assertions;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;
    public Text GameModeText;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true; // when the host starts the game, all the players will load the same scene as the host 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("PlayerName is invalid!");
        }
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }
    
    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }
            RoomOptions roomOptions = new RoomOptions();
            //Since the game has different game modes, we need to use custom properties 
            string[] roomPropertiesInLobby = { "gm" }; // gm = game mode 
                                                       // the game mode doesn't contain anything right now
                                                       // Using a hashtable from Photon 
                                                       // Hashtable is like a dictionary holds a key - value pair 
                                                       // "gm" will be the key and the value will be the game mode selected 
                                                       //Game Modes 
                                                       // rc = racing mode 
                                                       // dr = death race mode 
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } }; //This will be the game mode by default


            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        GameMode = gameMode;
        //Join a room that is based on the game mode selected
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", gameMode } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);

    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            // If RC is selected, scene change to racing mode 
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                PhotonNetwork.LoadLevel("RacingScene");
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr")) // if DR is selected, scene change to racing mode
            {
                PhotonNetwork.LoadLevel("DeathRaceScene");
            }
        }
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ " is connected to Photon");
        ActivatePanel(GameOptionsUIPanel.name);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " has been created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        ActivatePanel(InsideRoomUIPanel.name);
        //Know what game mode is selected
        object gameModeName;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName)){
            Debug.Log(gameModeName.ToString());
            RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
            {
                GameModeText.text = "Racing Mode";
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
            {
                GameModeText.text = "Death Race Mode";
            }
        }
       
        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            Assert.IsNotNull(PlayerListPrefab, "PlayerListPrefab is null or empty");
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            playerListItem.transform.SetParent(PlayerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;
            playerListItem.GetComponent<PlayerListItemInitalizer>().Initialize(player.ActorNumber, player.NickName); // Player.ActorNumber is the identifier for the player inside the room 

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInitalizer>().SetPlayerReady((bool)isPlayerReady);
            }
            playerListGameObjects.Add(player.ActorNumber, playerListItem);
        
        }

        StartGameButton.SetActive(false);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // called when new player joins the room
    {
        //when a player joins the room, create a new player list prefab and add it to the dictionary updating the current list
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        playerListItem.transform.SetParent(PlayerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;
        playerListItem.GetComponent<PlayerListItemInitalizer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName); // Player.ActorNumber is the identifier for the player inside the room 
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListItem);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // called when a player leaves the room
    {
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);
        foreach(GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Room " + Random.Range(1000, 10000);
            }
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 3;
            //Since the game has different game modes, we need to use custom properties 
            string[] roomPropertiesInLobby = { "gm" }; // gm = game mode 
                                                       // the game mode doesn't contain anything right now
                                                       // Using a hashtable from Photon 
                                                       // Hashtable is like a dictionary holds a key - value pair 
                                                       // "gm" will be the key and the value will be the game mode selected 
                                                       //Game Modes 
                                                       // rc = racing mode 
                                                       // dr = death race mode 
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } }; //This will be the game mode by default


            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) // called whenever a property of the player is changed
    {
        GameObject playerListGameObject;
        if(playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if(changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGameObject.GetComponent<PlayerListItemInitalizer>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient) // called when the original master client left the room and a new master client has been chosen 
    {
       if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }
    
    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods

    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if(p.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady) { 
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
}
