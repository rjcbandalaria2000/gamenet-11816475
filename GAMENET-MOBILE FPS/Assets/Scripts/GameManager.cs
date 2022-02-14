using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int RandomXPos = Random.Range(-10, 10);
            int RandomZPos = Random.Range(-10, 10);
            Vector3 spawnPoint = new Vector3(RandomXPos, 0, RandomZPos);
            PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(RandomXPos, 0, RandomZPos), Quaternion.identity);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
