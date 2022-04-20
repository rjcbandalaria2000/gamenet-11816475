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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject blasterFire = PhotonNetwork.Instantiate(ProjectilePrefab.name, FirePoint.position, FirePoint.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
