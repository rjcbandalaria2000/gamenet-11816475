using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; // Handles the room and lobbies from in Proton (creating rooms) 
using UnityEngine.Assertions;

public class LauncherManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject enterNamePanel;
    [SerializeField]
    GameObject connectionStatusPanel;
    [SerializeField]
    GameObject lobbyPanel;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // every client or player will load the scene the master client has loaded 
    }
    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings(); // Used to connect to Photon Servers 
        Assert.IsNotNull(enterNamePanel, "Enter name panel is null");
        enterNamePanel.SetActive(true);
        Assert.IsNotNull(connectionStatusPanel, "Connection Status Panel is null");
        connectionStatusPanel.SetActive(false);
        Assert.IsNotNull(lobbyPanel, "Lobby panel is null");
        lobbyPanel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        // Called whenever the game is connected to Photon server 
        Debug.Log(PhotonNetwork.NickName + " is connected to Photon Servers");
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    public override void OnConnected()
    {
        // Called to check whenever player is connected to the internet 
        Debug.Log("Connected to the Internet");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Called when joining a random room failed or there is no room to join
        Debug.LogWarning(message);
        CreateAndJoinRandomRoom();
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected) // Checks if connected to Photon Server
        { 
            PhotonNetwork.ConnectUsingSettings(); // Used to connect to Photon Servers
            enterNamePanel.SetActive(false);
            connectionStatusPanel.SetActive(true);

        }
    }

    // Rooms are organized in lobbies 
    public void JoinRandomRoom() 
    {
        PhotonNetwork.JoinRandomRoom(); //Used to join random rooms in Photon
    }
    public void CreateAndJoinRandomRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000); // Creates random room name
        RoomOptions roomOptions = new RoomOptions(); // Create random room object
        roomOptions.IsOpen = true; //Room is joinable
        roomOptions.IsVisible = true; //Room is visible to players 
        roomOptions.MaxPlayers = 20; // Sets max players in the room
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions); // Creates room
    }

    public override void OnJoinedRoom()
    {
        //Called when player has entered a room 
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel(1); // Loading level "GameScene" 
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name + 
            ". Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " players");
    }
}
