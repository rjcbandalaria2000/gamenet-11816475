using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerMovement PlayerMovement; 
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerMovement == null)
        {
            PlayerMovement = this.GetComponent<PlayerMovement>();
        }
        PlayerMovement.enabled = photonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
