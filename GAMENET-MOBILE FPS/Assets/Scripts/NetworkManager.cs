using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Assertions;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text connectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject loginUIPanel;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject showRoomListPanel;

    [Header("Inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    public GameObject playerListViewParent;
    public GameObject startGameButton;

    [Header("Room List Panel")]
    public GameObject roomListPanel;

    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList; // Dictionary accepts 2 objects, <key, value>
    //Each element in the dictionary will be stored with a string (the room name), value will be the roomInfo object
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects; 
    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    { 
        ActivatePanel(loginUIPanel);    
        cachedRoomList = new Dictionary<string, RoomInfo>();   
        roomListGameObjects = new Dictionary<string, GameObject>(); 
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState; // PhotonNetwork.NetworkClientState shows the connection status of the player 
    }
    #endregion

    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player Name is Invalid");
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName= playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        } 
        
        RoomOptions roomOptions = new RoomOptions();
        int roomMaxPlayers = int.Parse(playerCountInputField.text);

        if(roomMaxPlayers <= 20 && roomMaxPlayers > 0)
        { 
            roomOptions.MaxPlayers = (byte)roomMaxPlayers;
        }
        else
        {
            roomOptions.MaxPlayers = 20;
            Debug.Log("Max of 20 players are allowed");
        }
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel);
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(gameOptionsPanel);
    }

    public void OnShowRoomButtonClicked()
    {
        if(!PhotonNetwork.InLobby) // checks if the player is in a lobby or not
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(showRoomListPanel);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region PUN Callbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to the Internet");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to Photon Servers");
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " has been created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel);

        Assert.IsNotNull(roomInfoText, "Room Info Text should not be null or empty");
        DisplayCurrentPlayersInRoom();
        
        //Initialize playerListGameObjects Dictionary
        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        // PhotonNetwork.PlayerList contains all the players in the room
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // called when a room listing is updated inside the lobby and return a list of all the room info in the lobby
    {
        ClearRoomListGameObjects(); //clears any existing room list game objects to avoid duplicates
        Debug.Log("Room Info has been updated");

        Assert.IsNotNull(startGameButton, "Start button should not be null or empty");
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient); //checks if the local player is the master client or the host of the room

        foreach(RoomInfo info in roomList)
        {
            Debug.Log("Room Name: " + info.Name);

            if(!info.IsOpen || !info.IsVisible || info.RemovedFromList) // check if the room is not open or invisible or removed in the list when full 
            {
                if (cachedRoomList.ContainsKey(info.Name)) // check room duplicates
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                //Update existing room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else 
                {
                    cachedRoomList.Add(info.Name, info); //cache the room list info
                }
                
            }
        }
        foreach(RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one; // to avoid scaling issues 

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player Count: " + info.PlayerCount 
                + " / " + info.MaxPlayers;

            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomClicked(info.Name));// adding an onclick listener programmatically 
            //Remember to remove the onclick listener on the inspector  
            roomListGameObjects.Add(info.Name, listItem); // cache the room list game objects 
        }
    }

    public override void OnLeftLobby()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " left the lobby");
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayCurrentPlayersInRoom();

        GameObject playerItem = Instantiate(playerListItemPrefab);
        playerItem.transform.SetParent(playerListViewParent.transform);
        playerItem.transform.localScale = Vector3.one;

        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    { 
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        DisplayCurrentPlayersInRoom();
        //Remove the playerList game object
        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);//Remove player from cached 

    }

    public override void OnLeftRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has left the room");

        ClearPlayerListGameObjects();
        ActivatePanel(gameOptionsPanel);
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        string roomName = "Room " + Random.Range(1, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        PhotonNetwork.CreateRoom(roomName, roomOptions);   
    }

    #endregion

    #region Private Methods

    private void DisplayCurrentPlayersInRoom()
    {
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + "- Current Player Count: " +
           PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    private void OnJoinRoomClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby(); 
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach(var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }
        roomListGameObjects.Clear();
    }

    private void ClearPlayerListGameObjects()
    {
        foreach (var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }
    #endregion

    #region Public Methods

    public void ActivatePanel(GameObject panelToBeActivated)
    { 
        Assert.IsNotNull(loginUIPanel, "Login UI Panel should not be null or empty");
        loginUIPanel.SetActive(panelToBeActivated.Equals(loginUIPanel));

        Assert.IsNotNull(gameOptionsPanel, "Game options panel should not be null or empty");
        gameOptionsPanel.SetActive(panelToBeActivated.Equals(gameOptionsPanel));

        Assert.IsNotNull(createRoomPanel, "Create Room Panel should not be null or empty");
        createRoomPanel.SetActive(panelToBeActivated.Equals(createRoomPanel));

        Assert.IsNotNull(joinRandomRoomPanel, "Join Random Room Panel should not be null or empty");
        joinRandomRoomPanel.SetActive(panelToBeActivated.Equals(joinRandomRoomPanel));

        Assert.IsNotNull(showRoomListPanel, "Show Room List Panel should not be null or empty");
        showRoomListPanel.SetActive(panelToBeActivated.Equals(showRoomListPanel));

        Assert.IsNotNull(insideRoomPanel, "Inside Room Panel should not be null or empty");
        insideRoomPanel.SetActive(panelToBeActivated.Equals(insideRoomPanel));

        Assert.IsNotNull(roomListPanel, "RoomListPanel should not be null or empty");
        roomListPanel.SetActive(panelToBeActivated.Equals(roomListPanel));
    }



    #endregion
}
