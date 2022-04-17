using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class DeathRaceManager : MonoBehaviourPunCallbacks
{
    public static DeathRaceManager Instance;

    public GameObject[] VehiclePrefabs;
    public Transform[] StartingPoints;

    public List<GameObject> Players = new List<GameObject>();
    public int AlivePlayers = 0;

    [Header("UI")]
    public TextMeshProUGUI TimerText;
    public GameObject KillFeedUIParent;
    public GameObject WinUI;
    public TextMeshProUGUI WinnerText; 

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
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Debug.Log("Selected Vehicle: " + (int)playerSelectionNumber);
                Vector3 instantiatePosition = StartingPoints[actorNumber - 1].position;
                GameObject player = PhotonNetwork.Instantiate(VehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
                Players.Add(player);
                AlivePlayers++;
            }
        }
        Debug.Log("Number of Players in Lobby" + PhotonNetwork.PlayerList.Length);
        WinUI.SetActive(false);
    }
    
    [PunRPC]
    public void SpawnVehicle(int actorNumber, int playerNumber)
    {
        GameObject drVehicle = PhotonNetwork.Instantiate(VehiclePrefabs[playerNumber].name, StartingPoints[actorNumber - 1].position, Quaternion.identity);
        Players.Add(drVehicle);
        Debug.Log("Spawned Vehicle: " + playerNumber);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public int CheckAlivePlayers()
    {
        int alivePlayerCount = 0;
        foreach(GameObject player in Players)
        {
            Shooting playerShooting = player.GetComponent<Shooting>();
            if (playerShooting)
            {
                if(playerShooting.CurrentHP > 0)
                {
                    alivePlayerCount+=1;
                }
            }
        }
        Debug.Log("Players alive: " + alivePlayerCount);
        return alivePlayerCount;
    }

    [PunRPC]
    public void OnGameOver(string winnerName)
    {
        WinUI.SetActive(true);
        WinnerText.text = "#1 " + winnerName;
    }

    public void DisplayWinningScreen(string winnerName)
    {
        if (this.photonView)
        {
            Debug.Log("there is photon view");
        }
        this.photonView.RPC("OnGameOver", RpcTarget.AllBuffered, winnerName);
    }
}
