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

    [Header("Room List Panel")]
    public GameObject roomListPanel;

    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList; // Dictionary accepts 2 objects, <key, value>
    //Each element in the dictionary will be stored with a string (the room name), value will be the roomInfo object
    private Dictionary<string, GameObject> roomListGameObjects;
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
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // called when a room listing is updated inside the lobby and return a list of all the room info in the lobby
    {
        ClearRoomListGameObjects(); //clears any existing room list game objects to avoid duplicates
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
    }

    #endregion

    #region Private Methods

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
