using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Assertions;
using TMPro;
public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject camera;
    [SerializeField]
    TextMeshProUGUI playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine) //if the photon view is the player, disable control movement in other spawned players 
        {
            transform.GetComponent<MovementController>().enabled = true;
            Assert.IsNotNull(camera, "Camera should not be null");
            camera.GetComponent<Camera>().enabled = true;
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            Assert.IsNotNull(camera, "Camera should not be null"); 
            camera.GetComponent<Camera>().enabled = false;
        }
        Assert.IsNotNull(playerNameText, "PlayerNameTextUI is needed to display player name");
        playerNameText.text = photonView.Owner.NickName; 
    }

   
}
