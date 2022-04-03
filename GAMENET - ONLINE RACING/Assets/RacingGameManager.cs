using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RacingGameManager : MonoBehaviour
{
    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPositions;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log("Selected Vehicle: " + (int) playerSelectionNumber);
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = StartingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
