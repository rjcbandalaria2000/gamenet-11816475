using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> LapTriggers = new List<GameObject>();
    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;

    //How to add listeners for the Photon events
    private void OnEnable()
    {
        //Callback for the Photon Event
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        //Check if the sent event (photonEvent) is for the WhoFinishedEventCode
        if(photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            //retrieve the data passed from the photonEvent
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string) data[0]; // data[0] is the nickname based on the data from the photonEvent
            finishOrder = (int)data[1];

            Debug.Log(nickNameOfFinishedPlayer + " has finished " + finishOrder);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject laps in RacingGameManager.Instance.LapTriggers)
        {
            LapTriggers.Add(laps);
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LapTriggers.Contains(other.gameObject))
        {
            int indexOfTrigger = LapTriggers.IndexOf(other.gameObject);
            LapTriggers[indexOfTrigger].SetActive(false);
        }
        if(other.gameObject.tag == "FinishTrigger")
        {
            //Trigger game finish
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().Camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        finishOrder++;
        string nickName = photonView.Owner.NickName;

        //Event data 
        object[] data = new object[] {nickName, finishOrder }; // data being passed for the event 

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false

        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);

    }
}
