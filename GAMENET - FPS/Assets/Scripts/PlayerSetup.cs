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
        //Add Photon view component as an identifier if the Player spawned is yours 
        //Add to Photon view the Player's transform to update it on the server under Observed Components (switch Observable Search to "Manual")
        // For perfomance reasons, Photon will automatically add Photon transform view to the player then add it under Observed Components. 
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
