using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class Shooting : MonoBehaviourPunCallbacks
{
    public int Damage;
    public Transform FirePoint;
    public bool canShoot;
    public GameObject KillNotifUIPrefab;

    [Header("Health")]
    public int CurrentHP;
    public int MaxHP;
    

    private string killerName; 

    public enum RaiseEventsCode
    {
        WhoDiedEventCode = 0,
        WhoWonEventCode = 1
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoDiedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string deadPlayerNickname = (string)data[0];
            int viewID = (int)data[1];
            killerName = (string)data[2];
            GameObject killNotif = Instantiate(KillNotifUIPrefab);
            killNotif.transform.SetParent(DeathRaceManager.Instance.KillFeedUIParent.transform);
            killNotif.transform.Find("KilledText").GetComponent<TextMeshProUGUI>().text = deadPlayerNickname;

        }
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoWonEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            killerName = (string)data[2];
            Debug.Log("Killer Name: " + killerName);
            DeathRaceManager.Instance.DisplayWinningScreen(killerName);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    public virtual void Shoot()
    {
        
    }


    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        CurrentHP -= damage;
        Debug.Log("Took damage: " + damage);
        if (CurrentHP <= 0)
        {
            Death();
            killerName = info.Sender.NickName;
        }
    }

    public void Death()
    {
        if (photonView.IsMine) {

            Debug.Log("Death");
            GetComponent<PlayerSetup>().Camera.transform.parent = null;
            GetComponent<VehicleMovement>().enabled = false;
            GetComponent<Shooting>().canShoot = false;
            //GetComponent<BoxCollider>().enabled = false;
            DeathRaceManager.Instance.AlivePlayers--;
            string deadPlayerNickName = photonView.Owner.NickName;
            int viewId = photonView.ViewID;

            object[] data = new object[] { deadPlayerNickName, viewId, killerName};

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };
            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };

            if(DeathRaceManager.Instance.AlivePlayers >= 1)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoDiedEventCode, data, raiseEventOptions, sendOptions);
            }
            if(DeathRaceManager.Instance.AlivePlayers <= 1)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoWonEventCode, data, raiseEventOptions, sendOptions);
            }
            
        }
        
    }


}
