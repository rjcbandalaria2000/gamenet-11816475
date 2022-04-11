using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MachineGunShooting : Shooting
{

    public ParticleSystem MuzzleFlash;
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
                            if (MuzzleFlash)
                            {
                                MuzzleFlash.Play();
                            }
                        }
                    }
                }
            }
        }
    }


    public override void Shoot()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if(other.gameObject.GetComponent<Projectile>().Source != this.gameObject)
            {
                int projectileDamage = other.gameObject.GetComponent<Projectile>().Damage;
                photonView.RPC("TakeDamage", RpcTarget.AllBuffered, projectileDamage);
                Destroy(other.gameObject);
                Debug.Log("Took damage from projectile: " + projectileDamage);
            }
            
        }
    }
}
