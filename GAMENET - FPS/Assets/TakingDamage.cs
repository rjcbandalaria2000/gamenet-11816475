using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TakingDamage : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Image healthBar;
    public float maxHP = 100;
    public float currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        Assert.IsNotNull(healthBar, "Health bar image is null and not displaying");
        healthBar.fillAmount = currentHP / maxHP;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("Player health: " + currentHP);
        healthBar.fillAmount = currentHP / maxHP;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Player Dead");
        if (photonView.IsMine) //check if the player is the owner of the character 
        {
            GameManager.instance.LeaveRoom();
        }
        
    }
    
}
