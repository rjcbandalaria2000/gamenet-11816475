using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public TMP_InputField PlayerNameInput;

    [Header("Connecting UI")]
    public GameObject ConnectingUIPanel;

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

    #endregion
}
