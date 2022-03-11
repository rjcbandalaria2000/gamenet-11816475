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
    //public List<Player> PlayerList = new List<Player>();
    public List<GameObject> GameObjectPlayerList = new List<GameObject>();
    public int RequiredPlayerKills = 5;
    public GameObject WinUIPanel;

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
        
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //PlayerList.Remove(otherPlayer);
        
    }

    public void CheckWinCondition()
    {
        if (GameObjectPlayerList.Count > 0)
        {
            foreach (GameObject player in GameObjectPlayerList)
            {
                Shooting playerShooting = player.GetComponent<Shooting>();
                if (playerShooting)
                {
                    if (playerShooting.PlayerKills >= RequiredPlayerKills)
                    {
                        WinUIPanel.SetActive(true);
                        WinUIPanel.transform.Find("WinnerText").GetComponent<TextMeshProUGUI>().text = "Winner: " + player.GetComponent<PhotonView>().Owner.NickName;
                    }
                }
            }
        }
        
    }
    
}
