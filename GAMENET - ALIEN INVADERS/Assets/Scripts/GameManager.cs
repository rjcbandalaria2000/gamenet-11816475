using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Transform> SpawnPoints = new List<Transform>();
    public List<GameObject> SpaceShipPrefabs = new List<GameObject>(); 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Photon is Ready");
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log("Selected Space Ship: " + (int)playerSelectionNumber);
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = SpawnPoints[actorNumber - 1].position;
                PhotonNetwork.Instantiate(SpaceShipPrefabs[(int)playerSelectionNumber].name, instantiatePosition, SpawnPoints[actorNumber - 1].rotation);
            }
            else
            {
                Debug.Log("No Custom Properties");
            }
        }
    }

  
}
