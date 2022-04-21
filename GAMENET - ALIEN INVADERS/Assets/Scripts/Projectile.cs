using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public Rigidbody Rb;
    public int Damage = 0;
    public int ProjectileSpeed = 10;
    public GameObject Source;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Rb == null)
        {
            Rb = this.GetComponent<Rigidbody>();
        } 
       
    }

    // Update is called once per frame
    void Update()
    {
        Rb.AddForce(this.transform.up * ProjectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            if(Source != other.gameObject.GetComponent<Projectile>().Source)
            {
                Destroy(this.gameObject);
                Destroy(other.gameObject);
                Debug.Log("Destroyed another projectile");
            }
           
        }
    }
}
