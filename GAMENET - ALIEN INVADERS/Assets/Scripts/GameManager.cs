using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public List<Transform> SpawnPoints = new List<Transform>();
    public List<GameObject> SpaceShipPrefabs = new List<GameObject>(); 
    public Dictionary<int , GameObject> PlayersInRoom = new Dictionary<int , GameObject>();

    [Header("UI")]
    public GameObject WinUI;

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
            }
            else
            {
                Debug.Log("No Custom Properties");
            }
            if (PhotonNetwork.IsMasterClient)
            {
                StartWave();
            }
        }
        
    }

    public void GameWon()
    {

    }
    
    [PunRPC]
    public void OnGameWon()
    {

    }

    public void StartWave()
    {
        spawnCoroutine = StartCoroutine(SpawnWave());
    }

    public void StartNextWave()
    {
        if (SpawnedEnemies <= 0)
        {
            if (WaveCount < WaveDatas.Count)
            {
                WaveCount++;
                StartWave();
            }
            else
            {
                Debug.Log("No More Waves");
            }
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
        int totalSpawnedEnemiesInWave = 0;
        for (int i = 0; i < WaveDatas[WaveCount].NumberOfEnemies.Count; i++)
        {
            totalSpawnedEnemiesInWave += WaveDatas[WaveCount].NumberOfEnemies[i];
        }
        Debug.Log("Total Spawned Enemies in Wave: " + totalSpawnedEnemiesInWave);
        
        if(SpawnedEnemies >= totalSpawnedEnemiesInWave)
        {
            Debug.Log("All Spawned");
        }
    }

    [PunRPC]
    public void RemoveEnemies()
    {
        //SpawnedEnemies--;
        DestroyedEnemies++;
        int totalSpawnedEnemiesInWave = 0;
        for (int i = 0; i < WaveDatas[WaveCount].NumberOfEnemies.Count; i++)
        {
            totalSpawnedEnemiesInWave += WaveDatas[WaveCount].NumberOfEnemies[i];
        }
        Debug.Log("Total Spawned Enemies in Wave: " + totalSpawnedEnemiesInWave);
        if (DestroyedEnemies >= totalSpawnedEnemiesInWave)
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
}
