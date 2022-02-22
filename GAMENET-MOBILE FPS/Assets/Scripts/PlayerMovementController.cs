using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick Joystick;
    public FixedTouchField FixedTouchField;
    private RigidbodyFirstPersonController RigidBodyFPSController;
    // Start is called before the first frame update
    void Start()
    {
        RigidBodyFPSController = this.GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        RigidBodyFPSController.JoystickInputAxis.x = Joystick.Horizontal;
        RigidBodyFPSController.JoystickInputAxis.y = Joystick.Vertical;
        RigidBodyFPSController.mouseLook.LookInputAxis = FixedTouchField.TouchDist;
    }
}
