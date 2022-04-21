using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerMovement PlayerMovement;
    public Shooting Shooting; 
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerMovement == null)
        {
            PlayerMovement = this.GetComponent<PlayerMovement>();
        }
        if(Shooting == null)
        {
            Shooting = this.GetComponent<Shooting>();
        }
        PlayerMovement.enabled = photonView.IsMine;
        //Shooting.enabled = photonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
