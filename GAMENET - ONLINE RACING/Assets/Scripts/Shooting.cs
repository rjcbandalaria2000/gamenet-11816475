using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public int Damage;
    public Transform FirePoint;
    public bool canShoot;

    [Header("Health")]
    public int CurrentHP;
    public int MaxHP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    public virtual void Shoot()
    {
        
    }


    [PunRPC]
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        Debug.Log("Took damage: " + damage);
        if (CurrentHP <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Debug.Log("Death");
    }
}
