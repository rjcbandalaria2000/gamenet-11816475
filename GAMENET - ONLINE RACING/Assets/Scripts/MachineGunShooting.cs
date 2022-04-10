using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MachineGunShooting : Shooting
{
    // Start is called before the first frame update
    void Start()
    {
        CurrentHP = MaxHP;
        canShoot = photonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RaycastHit hit;
                if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit))
                {
                    if (!hit.transform.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        Shooting hitShooting = hit.transform.gameObject.GetComponent<Shooting>();
                        if (hitShooting)
                        {
                            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, Damage);
                        }
                    }
                }
            }
        }
    }


    public override void Shoot()
    {
        
    }
}
