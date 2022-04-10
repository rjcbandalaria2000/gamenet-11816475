using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks
{
    public int CurrentHealth;
    public int MaxHealth; 
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }
    
    [PunRPC]
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            Death();
        }
    }

    public void Death()
    {

    }
    
}
