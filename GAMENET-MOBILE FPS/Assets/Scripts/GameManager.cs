using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public GameObject PlayerPrefab;
    public int RequiredPlayerKills = 5;
    public GameObject WinUIPanel;
    public TextMeshProUGUI WinnerNameUI; 
    public Dictionary<Player, int> PlayersInGame = new Dictionary<Player, int>();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            //int RandomXPos = Random.Range(-10, 10);
            //int RandomZPos = Random.Range(-10, 10);
            //Vector3 spawnPoint = new Vector3(RandomXPos, 0, RandomZPos);
            PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnManager.Instance.GetRandomSpawnPoint().position, Quaternion.identity);
            
        }
        WinUIPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player added in Player List: " + newPlayer.NickName);
        //PlayersInGame.Add(newPlayer,  )
       
        //Debug.Log("Name " + photonView.GetComponent<Shooting>().PlayerKills);

        //PlayersInGame.Add(newPlayer, photonView.GetComponent<Shooting>().PlayerKills);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //PlayerList.Remove(otherPlayer);
        
    }

    [PunRPC]
    public void OnGameOver(string winnerName)
    { 
       WinUIPanel.SetActive(true); 
       WinnerNameUI.text = "Winner: " + winnerName;
    }

    public void DisplayGameOverUI(string winnerName)
    {
        if (this.photonView)
        {
            Debug.Log("Photon view exists");
        }
        this.photonView.RPC("OnGameOver", RpcTarget.AllBuffered, winnerName);
    }
    
}
