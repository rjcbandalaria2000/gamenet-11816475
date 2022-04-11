using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{
    public Rigidbody Rb;
    public int Speed;
    public float Duration = 2f;
    public int Damage;
    public GameObject Source;
    // Start is called before the first frame update
    void Start()
    {
        Rb = this.GetComponent<Rigidbody>();
        Destroy(this.gameObject, Duration);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Rb.AddForce(this.transform.forward * Speed * Time.deltaTime);
    }

   
}
