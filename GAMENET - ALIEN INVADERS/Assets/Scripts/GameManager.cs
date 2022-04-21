using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public List<Transform> SpawnPoints = new List<Transform>();
    public List<GameObject> SpaceShipPrefabs = new List<GameObject>(); 
    public Dictionary<int , GameObject> PlayersInRoom = new Dictionary<int , GameObject>();
    public List<GameObject> Players = new List<GameObject>();
    public Dictionary<int, int> GameScores = new Dictionary<int, int>();    
    
    [Header("UI")]
    public GameObject WinUI;
    public List<GameObject> FinisherUIs = new List<GameObject>();
    public int FinisherUIIndex = 0;
    //public List<TextMeshProUGUI> PlayerScores = new List<TextMeshProUGUI>();
    public TextMeshProUGUI WinnerText;
    public TextMeshProUGUI SecondWinnerText;

    [Header("Spawner")]
    public List<WaveData> WaveDatas = new List<WaveData>();
    public Spawner Spawner;
    public int WaveCount = 0;
    public int SpawnedEnemies;
    public int DestroyedEnemies;

    private Coroutine spawnCoroutine; 

    public enum RaiseEventsCode
    {
        OnStartWave = 0,
        WhoWon = 1
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventsCode.OnStartWave)
        {

        }
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoWon)
        {
            object[] data = (object[])photonEvent.CustomData;

            string winnerName = (string)data[0];
            //string secondPlaceName = (string)data[1];
            int winnerPoints = (int)data[1];
            //int secondPlacePoints = (int)data[3];

            WinUI.SetActive(true);
            FinisherUIs[FinisherUIIndex].GetComponent<TextMeshProUGUI>().text = winnerName + " Points: " + winnerPoints.ToString();
            FinisherUIIndex++;
            //SecondWinnerText.text = "#2 " + secondPlaceName + " Points: " + secondPlacePoints.ToString();
        }
    }

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
                Debug.Log("Actor Number: " + PhotonNetwork.LocalPlayer.ActorNumber);
                Vector3 instantiatePosition = SpawnPoints[actorNumber - 1].position;
                GameObject player = PhotonNetwork.Instantiate(SpaceShipPrefabs[(int)playerSelectionNumber].name, instantiatePosition, SpawnPoints[actorNumber - 1].rotation);
                PlayersInRoom.Add(PhotonNetwork.LocalPlayer.ActorNumber, player);
                Players.Add(player);
                Debug.Log("Players in Lobby " + PlayersInRoom.Count);
            }
            else
            {
                Debug.Log("No Custom Properties");
            }
            if (PhotonNetwork.IsMasterClient)
            {
                StartWave();
            }
            WinUI.SetActive(false);
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       Debug.Log("New Player: " + newPlayer.NickName);
    }

    [PunRPC]
    public void RecordScores(int actorNumber , int points)
    {
        GameScores.Add(actorNumber, points);
      
    }


    public void PhotonRecordScores(int actorNumber, string name, int points)
    {
        photonView.RPC("RecordScores", RpcTarget.AllBuffered, actorNumber, name , points);  
    }

    public void GameWon()
    {
        photonView.RPC("OnGameWon", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OnGameWon()
    {
        string winnerName = GetPlayerGameObject(PhotonNetwork.LocalPlayer.ActorNumber).GetComponent<PhotonView>().Owner.NickName;
        int winnerPoints = GetPlayerGameObject(PhotonNetwork.LocalPlayer.ActorNumber).GetComponent<Shooting>().Points;

       
     

      

       


        object[] data = new object[]
        {
            winnerName, winnerPoints
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };
        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoWon, data, raiseEventOptions, sendOptions);
        Debug.Log("Game Finished");
    }

    

    public void StartWave()
    {
        spawnCoroutine = StartCoroutine(SpawnWave());
    }

    public void StartNextWave()
    {
        SpawnedEnemies = 0;
        DestroyedEnemies = 0;
        WaveCount++;
        if (WaveCount < WaveDatas.Count)
        {
            
            StartWave();
        }
        else
        {
            Debug.Log("No More Waves");
            GameWon();
        }
        
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(WaveDatas[WaveCount].PrepTime);
        Spawner.EnemyPrefabs = WaveDatas[WaveCount].EnemiesToSpawn;
        Spawner.EnemyCountToSpawn = WaveDatas[WaveCount].NumberOfEnemies;
        Spawner.SpawnEnemies();
    }

    [PunRPC]
    public void AddEnemies()
    {
        SpawnedEnemies++;
        
        if(SpawnedEnemies >= GetTotalSpawnedEnemiesinWave())
        {
            Debug.Log("All Spawned");
        }
    }

    [PunRPC]
    public void RemoveEnemies()
    {
        //SpawnedEnemies--;
        DestroyedEnemies++;

        if (DestroyedEnemies >= GetTotalSpawnedEnemiesinWave())
        {
            Debug.Log("Spawn next wave");
            StartNextWave();
        }
    }

    public void PhotonAddEnemies()
    {
        photonView.RPC("AddEnemies", RpcTarget.AllBuffered);
    }

    public void PhotonRemoveEnemies()
    {
        photonView.RPC("RemoveEnemies", RpcTarget.AllBuffered);
    }
    
    public GameObject GetPlayerGameObject(int actorNumber)
    {
        GameObject player; 
        PlayersInRoom.TryGetValue(actorNumber, out player);
        return player;
    }

    public int GetTotalSpawnedEnemiesinWave()
    {
        int totalSpawnedEnemiesInWave = 0;
        if(WaveCount < WaveDatas.Count)
        {
            for (int i = 0; i < WaveDatas[WaveCount].NumberOfEnemies.Count; i++)
            {
            totalSpawnedEnemiesInWave += WaveDatas[WaveCount].NumberOfEnemies[i];
            }
        }
        
        return totalSpawnedEnemiesInWave;
    }
}
