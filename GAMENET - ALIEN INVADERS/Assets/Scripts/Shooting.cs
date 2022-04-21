using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public GameObject Source;
    public GameObject ProjectilePrefab;
    public Transform FirePoint;
    public int Damage = 1;
    public bool canShoot;

    [Header("Player Points")]
    public int Points = 0;

    // Start is called before the first frame update
    void Start()
    {
        canShoot = photonView.IsMine;
        if(Source == null)
        {
            Source = this.gameObject;
        }
    }

    private void Update()
    {
        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
    }

    [PunRPC]
    public void Shoot()
    {
        GameObject blasterFire = PhotonNetwork.Instantiate(ProjectilePrefab.name, FirePoint.position, FirePoint.rotation);
        Projectile blasterProjectile = blasterFire.GetComponent<Projectile>();
        if (blasterProjectile)
        {
            Debug.Log("There is projectile");
            blasterProjectile.Source = Source;
            Debug.Log("There is source");
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
