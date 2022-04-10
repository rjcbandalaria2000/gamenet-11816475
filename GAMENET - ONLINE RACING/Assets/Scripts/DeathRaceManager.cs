using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DeathRaceManager : MonoBehaviour
{
    public static DeathRaceManager Instance;

    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPoints;

    public List<GameObject> Players = new List<GameObject>();

    public TextMeshProUGUI TimerText;

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
        if (PhotonNetwork.IsConnected)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Debug.Log("Selected Vehicle: " + (int)playerSelectionNumber);
                Vector3 instantiatePosition = StartingPoints[actorNumber - 1].position;
                PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            
            }
        }
        Debug.Log("Number of Players in Lobby" + PhotonNetwork.PlayerList.Length);
    }
    
    [PunRPC]
    public void SpawnVehicle(int actorNumber, int playerNumber)
    {
        GameObject drVehicle = PhotonNetwork.Instantiate(VehiclePrefabs[playerNumber].name, StartingPoints[actorNumber - 1].position, Quaternion.identity);
        Players.Add(drVehicle);
        Debug.Log("Spawned Vehicle: " + playerNumber);
    }

   
}
