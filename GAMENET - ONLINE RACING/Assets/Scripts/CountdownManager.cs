using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI TimerText;
    public float TimeToStartRace = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            TimerText = RacingGameManager.Instance.TimerText;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            TimerText = DeathRaceManager.Instance.TimerText;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient) // only the master client can start the countdown 
        { 
            if (TimeToStartRace > 0)
            {
                TimeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, TimeToStartRace);
            }
            else if (TimeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            TimerText.text = time.ToString("F1");
        }
        else
        {
            TimerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlledEnabled = true;
        this.enabled = false;
    }
}
