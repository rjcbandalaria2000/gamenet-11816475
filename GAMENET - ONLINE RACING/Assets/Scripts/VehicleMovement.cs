using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public float Speed = 20f;
    public float RotationSpeed = 200f;
    public float CurrentSpeed = 0f;

    public bool isControlledEnabled;
    // Start is called before the first frame update
    void Start()
    {
        isControlledEnabled = false;
    }

    private void LateUpdate()
    {
        if (isControlledEnabled)
        {
            float translation = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            CurrentSpeed = translation;
            transform.Rotate(0, rotation, 0);
        }
    }
}
