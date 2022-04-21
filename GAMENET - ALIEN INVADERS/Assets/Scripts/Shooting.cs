using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public GameObject ProjectilePrefab;
    public Transform FirePoint;
    public int Damage = 1;

    [Header("Health")]
    public int CurrentHP;
    public int MaxHP;

    [Header("Player Points")]
    public int Points = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject blasterFire = PhotonNetwork.Instantiate(ProjectilePrefab.name, FirePoint.position, FirePoint.rotation);
            blasterFire.GetComponent<Projectile>().Source = this.gameObject;
        }
    }
    [PunRPC]
    public void AddPoints(int pointValue)
    {
        Points += pointValue;
        Debug.Log("Gained points. Total Points: " + Points);
    }

    
    public void GainPoints(int pointValue)
    {
        this.photonView.RPC("AddPoints", RpcTarget.AllBuffered, pointValue);
    }

}
