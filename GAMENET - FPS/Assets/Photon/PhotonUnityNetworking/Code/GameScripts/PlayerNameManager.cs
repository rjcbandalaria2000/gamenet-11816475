using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
   public void SetPlayerName(string name)
    {
        if (name == null)
        {
            Debug.LogWarning("Player name cannot be empty");
            return;
        }
        PhotonNetwork.NickName = name;

    }
}
