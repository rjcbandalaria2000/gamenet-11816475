using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

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


    #endregion
}
