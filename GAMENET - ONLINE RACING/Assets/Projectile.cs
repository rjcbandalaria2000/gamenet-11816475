using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{
    public Rigidbody Rb;
    public int Speed;
    public float Duration = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, Duration);
        Rb = this.GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Rb.velocity = Vector3.right * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
