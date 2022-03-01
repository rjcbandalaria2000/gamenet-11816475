using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject FPSModel;
    public GameObject NonFPSModel;
    public GameObject PlayerUIPrefab;
    public PlayerMovementController MovementController;
    public Camera FPSCamera;
    public Avatar NonFPSAvatar;
    public Avatar FPSAvatar; 

    private Animator Animator;
    private Shooting shooting;

    // Start is called before the first frame update
    void Start()
    {
        MovementController = this.GetComponent<PlayerMovementController>();
        Assert.IsNotNull(FPSModel, "No FPSModel set or is null");
        //Enable FPS Model if the Photon View is the player 
        FPSModel.SetActive(photonView.IsMine);
        Assert.IsNotNull(NonFPSModel, "No NonFPSModel set or is null");
        //Enable the non FPS model if the photon view is not the player 
        NonFPSModel.SetActive(!photonView.IsMine);
        Animator = this.GetComponent<Animator>();
        Assert.IsNotNull(Animator, "No Animator set or is null");
        Animator.SetBool("IsLocalPlayer", photonView.IsMine);

        Animator.avatar = photonView.IsMine ? FPSAvatar : NonFPSAvatar;
        //Set FPSAvatar if the photonView is the player : Set NonFPSAvatar if not the player 
        
        shooting = this.GetComponent<Shooting>();
        
        if (photonView.IsMine)
        {   // If the Photon View is the player, spawn in the PlayerUI (Joysticks), enable movement and camera 
            Assert.IsNotNull(PlayerUIPrefab, "PlayerUIPrefab not set or is null");
            GameObject PlayerUI = Instantiate(PlayerUIPrefab);
            Assert.IsNotNull(MovementController, "PlayerMovementController not set or is null");
            MovementController.FixedTouchField = PlayerUI.transform.Find("RotationTouchField ").GetComponent<FixedTouchField>();
            MovementController.FixedTouchField = PlayerUI.transform.Find("RotationTouchField ").GetComponent<FixedTouchField>();
            Assert.IsNotNull(MovementController.FixedTouchField, "No Fixed Touch Field Found");
            MovementController.Joystick = PlayerUI.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();
            Assert.IsNotNull(MovementController.Joystick, "No Joystick Found");

            Assert.IsNotNull(FPSCamera, "No Camera detected in the Player");
            FPSCamera.enabled = true;

            PlayerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            // Else disable the movement controller 
            MovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            FPSCamera.enabled = false;
        }

        /* Photon View: Synchronization
         * For fast data change, set it to Unreliable on Change like for movement for the server to compromise for other data to synchronize even if there is lag. 
         */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
