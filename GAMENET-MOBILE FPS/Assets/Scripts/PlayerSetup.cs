using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Assertions;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject FPSModel;
    public GameObject NonFPSModel;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(FPSModel, "No FPSModel set or is null");
        //Enable FPS Model if the Photon View is the player 
        FPSModel.SetActive(photonView.IsMine);
        Assert.IsNotNull(NonFPSModel, "No NonFPSModel set or is null");
        //Enable the non FPS model if the photon view is not the player 
        NonFPSModel.SetActive(!photonView.IsMine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
