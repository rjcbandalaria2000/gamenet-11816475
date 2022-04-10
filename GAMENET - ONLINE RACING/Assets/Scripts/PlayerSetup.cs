using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera Camera;
    // Start is called before the first frame update
    void Start()
    {
        this.Camera = transform.Find("Camera").GetComponent<Camera>();  
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            //Enable only if the owner of photonView is the player
            this.GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            Debug.Log(photonView.Owner.NickName + photonView.IsMine.ToString());
            GetComponent<LapController>().enabled = photonView.IsMine;
            Camera.enabled = photonView.IsMine;
            
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            this.GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            Camera.enabled = photonView.IsMine;
        }
    }

  
}
