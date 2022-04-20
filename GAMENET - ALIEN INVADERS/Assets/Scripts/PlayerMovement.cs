using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 50;
    public Rigidbody Rb;
    public bool isControllerEnabled;
    private Vector3 movement; 

    public void Awake()
    {
        if (Rb == null)
        {
            Rb = this.GetComponent<Rigidbody>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        movement.x = Input.GetAxis("Horizontal");

        Rb.MovePosition(Rb.position + movement * MoveSpeed * Time.fixedDeltaTime);
    }
}
