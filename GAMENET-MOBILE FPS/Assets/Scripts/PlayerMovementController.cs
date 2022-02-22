using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick Joystick;
    public FixedTouchField FixedTouchField;

    private RigidbodyFirstPersonController RigidBodyFPSController;
    private Animator Animator; 
    // Start is called before the first frame update
    void Start()
    {
        RigidBodyFPSController = this.GetComponent<RigidbodyFirstPersonController>();
        Animator = this.GetComponent<Animator>();
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
        //For animator movement 
        Animator.SetFloat("Horizontal", Joystick.Horizontal);
        Animator.SetFloat("Vertical", Joystick.Vertical);
        if(Mathf.Abs(Joystick.Horizontal) > 0.9 || Mathf.Abs(Joystick.Vertical) > 0.9)
        {
            Animator.SetBool("IsRunning", true);
            RigidBodyFPSController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            Animator.SetBool("IsRunning", false);
            RigidBodyFPSController.movementSettings.ForwardSpeed = 5;
        }
    }
}
