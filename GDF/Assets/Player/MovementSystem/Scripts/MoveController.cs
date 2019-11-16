using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// The big cheese, this is, as the name implies, the movemenmt controller and responsible for how the player moves in the world. It has in-built controller support (although, partial)
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(StateController), typeof(LookController))]
public class MoveController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private float crouchSpeedDivider;
    [SerializeField]
    private int numStepSounds;
    [SerializeField]
    private int airControlMultiplier;
    [SerializeField]
    private bool airControlSeparate;
    [SerializeField]
    private float crouchHeight;
    [SerializeField]
    private bool _canMove = true;

    private StateController _sc;
    private Rigidbody _rb;
    private CapsuleCollider _coll;
    private Camera _cam;
    private bool _isGrounded;
    private bool _jumpCooled = true;
    private float _timeSinceStep;
    private bool _isMoving = false;
    private float _moveSpeed;
    private float _standingHeight;
    

    //====
    //Unity Calls
    //====
    private void Start()
    {
        SetupRigidbody();
        SetupCollider();
        SetupCamera();
        SetupStateController();
        SetupMouse();
        SetupStandingHeight();
    }

    private void Update()
    {
        if (_canMove)
        {
            ProcessMovement();
            ProcessJump();
            ProcessGround();
            ProcessSteps();
            ProcessCrouching();
        }
        else
        {
            ApplyMovement(0, 0);
        }
    }

    //====
    //Private Calls
    //====

    //Setup
    //-----
    #region Setup
    private void SetupRigidbody()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.freezeRotation = true;
    }

    private void SetupCollider()
    {
        _coll = GetComponent<CapsuleCollider>();
    }

    private void SetupCamera()
    {
        _cam = Camera.main;
    }

    private void SetupStandingHeight()
    {
        _standingHeight = _coll.height;
    }

    private void SetupMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetupStateController()
    {
        _sc = GetComponent<StateController>();
    }

    #endregion

    //X and Z Movement
    //----------------
    #region XandZMovement

    private void ProcessMovement()
    {
        InputDevice device = InputManager.ActiveDevice;

        if (device.Name == "None") //OPTION: Make a setting for "Don't Use Controller" or figure out if they're trying to use it anyway
        {
            ProcessKeyboardMovement();
        }
        else
        {
            ProcessControllerMovement(device);
        }
    }

    private void ProcessControllerMovement(InputDevice device)
    {
        ApplyMovement(device.LeftStickY, device.LeftStickX);
    }

    private void ProcessKeyboardMovement()
    {
        ApplyMovement(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    }

    private void ApplyMovement(float y, float x)
    {
        Vector3 movement = Vector3.zero;
        Vector3 forwardMovement = Vector3.zero;
        Vector3 lateralMovement = Vector3.zero;

        forwardMovement = (transform.forward * y) * speed;

        lateralMovement = (transform.right * x) * speed;

        movement = forwardMovement + lateralMovement;

        if (_sc.state == StanceState.Crouching)
        {
            if (crouchSpeedDivider == 0)
            {
                Debug.LogError("Crouch Speed Divider MUST NOT BE 0");
            }

            movement = movement / crouchSpeedDivider;
        }

        if (airControlSeparate)
        {
            if (_isGrounded == false)
            {
                movement = movement * airControlMultiplier;

                AddForce(movement);
            }
            else
            {
                _sc.speed = movement.magnitude;

                SetVelocityWithGravity(movement);
            }
        }
        else
        {
            _sc.speed = movement.magnitude;

            SetVelocityWithGravity(movement);
        }
    }

    #endregion

    //Jumping (Y Movement)
    //--------------------
    #region Jumping

    private void ProcessJump()
    {
        InputDevice device = InputManager.ActiveDevice;

        if (device.Name == "None") //OPTION: Make a setting for "Don't Use Controller" or figure out if they're trying to use it anyway
        {
            ProcessKeyboardJump();
        }
        else
        {
            ProcessControllerJump(device);
        }
    }

    private void ProcessKeyboardJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            ApplyJump();
        }
    }

    private void ProcessControllerJump(InputDevice device)
    {
        if (device.Action1)
        {
            ApplyJump();
        }
    }

    private void ApplyJump()
    {
        if (_isGrounded && _jumpCooled && _sc.state == StanceState.Standing)
        {
            _rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            _isGrounded = false;
            _jumpCooled = false;
            Invoke("CoolJump", 0.3f);
        }
    }

    private void CoolJump()
    {
        _jumpCooled = true;
    }

    #endregion

    //Crouching (Other Y Movement)
    //----------------------------
    #region Crouching

    private void ProcessCrouching()
    {
        InputDevice device = InputManager.ActiveDevice;

        if (device.Name == "None") //OPTION: Make a setting for "Don't Use Controller" or figure out if they're trying to use it anyway
        {
            ProcessKeyboardCrouching();
        }
        else
        {
            ProcessControllerCrouching(device);
        }

        ApplyCrouch();
    }

    private void ProcessKeyboardCrouching()
    {
        if (Input.GetKey(KeyCode.C) && _isGrounded)
        {
            _sc.state = StanceState.Crouching;
        }
        else
        {
            _sc.state = StanceState.Standing;
        }
    }

    private void ProcessControllerCrouching(InputDevice device)
    {
        if (device.Action2 && _isGrounded)
        {
            _sc.state = StanceState.Crouching;
        }
        else
        {
            _sc.state = StanceState.Standing;
        }
    }

    private void ApplyCrouch()
    {
        if (_sc.state == StanceState.Crouching)
        {
            _coll.height = crouchHeight;
        }
        else
        {
            _coll.height = _standingHeight;
        }
    }

    #endregion

    //Utilities
    //---------
    #region Utilities

    private void AddForce(Vector3 movement)
    {
        _rb.AddForce(movement);
    }

    private void SetVelocity(Vector3 velocity)
    {
        _rb.velocity = velocity;
    }

    private void AddVelocity(Vector3 velocity)
    {
        SetVelocity(_rb.velocity + velocity);
    }

    private void SetVelocityWithGravity(Vector3 velocity)
    {
        Vector3 newVelocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);

        SetVelocity(newVelocity);
    }

    #endregion

    //Ground Control (to major tom)
    //-----------------------------
    #region GroundControl

    private void ProcessGround()
    {
        Ray billy = new Ray(this.transform.position, Vector3.down);
        RaycastHit hitInfo;

        if (Physics.Raycast(billy, out hitInfo, 1.5f))
        {
            if (hitInfo.collider.gameObject.GetComponent<JumpableObject>() != null && _jumpCooled)
            {
                _isGrounded = true;
            }
            else
            {
                Debug.DrawRay(this.transform.position, Vector3.down, Color.red);
            }
        }
        else
        {
            _isGrounded = false;
        }
    }

    #endregion

    //Audio Stuff
    //-----------
    #region AudioStuff

    private void ProcessSteps()
    {
        if(_isMoving)
        {
            _timeSinceStep += Time.deltaTime;
            float timeThreshold = 1.6f - _moveSpeed;

            if(_timeSinceStep > timeThreshold)
            {
                PlayStep();
                _timeSinceStep = 0;
            }
        }
    }

    private void PlayStep(bool isCrouching = false)
    {
        //select a random footstep sound.
        //select a random pitch
        //play the sound at our feet
    }

    private void PlayCrawl()
    {
        //select random sound
        //select random pitch
        //play sound
    }

    private void PlayJump()
    {
        //select random pitch
        //play the sound at our feet
    }

    private void PlayLand()
    {
        //select random pitch
        //play the sound at our feet
    }

    #endregion


    //=============
    //Public Calls
    //=============
    #region PublicCalls

    public void SetMovementEnabled(bool canMove)
    {
        _canMove = canMove;
    }
    #endregion
}
