using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;
    public int PlayerSelectionNumber;
    // Start is called before the first frame update
    void Start()
    {
        PlayerSelectionNumber = 0; 
        ActivatePlayer(PlayerSelectionNumber);
    }

    private void ActivatePlayer(int playerNumber)
    {
        foreach (GameObject selectablePlayer in SelectablePlayers)
        {
            selectablePlayer.SetActive(false);
        }
        SelectablePlayers[playerNumber].SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { Constants.PLAYER_SELECTION_NUMBER, PlayerSelectionNumber }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }
    public void GoToNextPlayer()
    {
        PlayerSelectionNumber++;
        if(PlayerSelectionNumber >= SelectablePlayers.Length)
        {
            PlayerSelectionNumber = 0;
        }
        ActivatePlayer(PlayerSelectionNumber);
    }
    public void GoToPreviousPlayer()
    {
        PlayerSelectionNumber--;
        if(PlayerSelectionNumber < 0)
        {
            PlayerSelectionNumber = SelectablePlayers.Length - 1;
        }
        ActivatePlayer(PlayerSelectionNumber);
    }

}
