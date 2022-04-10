using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileShooting : Shooting
{
    public GameObject Projectile;

    // Start is called before the first frame update
    void Start()
    {
        canShoot = photonView.IsMine;   
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                Shoot();
            }
        }
       
    }

    public override void Shoot()
    {
        GameObject spawnedProjectile = PhotonNetwork.Instantiate(Projectile.name, FirePoint.position, FirePoint.rotation) ;

    }
}
