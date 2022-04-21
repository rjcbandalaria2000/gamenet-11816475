using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPoints : MonoBehaviourPunCallbacks
{
    public int CurrentHealth;
    public int MaxHealth;

    [Header("Points")]
    public int PointValue;
    public GameObject Killer;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        CurrentHealth -= damage;
        Debug.Log("Hit by: " + info.Sender.NickName);
        
        if(CurrentHealth <= 0)
        {
            Debug.Log("Death");
            Killer = GameManager.Instance.GetPlayerGameObject(info.Sender.ActorNumber);
            Death();
        }
    }

    public void Death()
    {
       
            Killer.GetComponent<Shooting>().GainPoints(PointValue);
            GameManager.Instance.PhotonRemoveEnemies();
            PhotonNetwork.Destroy(this.gameObject);   
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            int projectileDamage = other.GetComponent<Projectile>().Damage;
            Killer = other.GetComponent<Projectile>().Source;
            if (Killer)
            {
                Debug.Log("Killer is set");
            }
            photonView.RPC("TakeDamage", RpcTarget.AllBuffered, projectileDamage);
            //Destroy(other.gameObject);
            
            
             Destroy(other.gameObject);
            
           
            Debug.Log("Hit by projectile");
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            GameManager.Instance.PhotonRemoveEnemies();
           
                PhotonNetwork.Destroy(this.gameObject);
            
            //PhotonNetwork.Destroy(this.gameObject);
        }
    }

}
