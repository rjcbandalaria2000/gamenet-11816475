using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody Rb;
    public int MoveSpeed = 50;
    public Vector3 Movement; 
    // Start is called before the first frame update
    void Start()
    {
        if(Rb == null)
        {
            Rb = this.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rb.AddForce(this.transform.right * MoveSpeed * Time.deltaTime);
    }
}
