using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Look controller responsible for moving the camera based on look. Works off the mouse or the controller right stick.
/// </summary>
public class LookController : MonoBehaviour
{
    //Things that will be options
    public float mouseSensitivity;
    //End things that will be options

    [SerializeField]
    private float maxLookUp;
    [SerializeField]
    private float maxLookDown;

    private Vector2 _camRotation = new Vector2(0,0);
    private Vector2 _playerRotation = new Vector2(0, 0);
    private Transform _playerTransform;

    //====
    //Unity Methods
    //====
    private void Start()
    {
        SetupPlayerTransform();
    }

    private void Update()
    {
        ProcessLook();
    }

    //====
    //Private Methods
    //====
    private void SetupPlayerTransform()
    {
        _playerTransform = GameObject.FindObjectOfType<MoveController>().transform;
    }
    private void ProcessLook()
    {
        InputDevice device = InputManager.ActiveDevice;

        if(device.Name == "None") //OPTION: Make a setting for "Don't Use Controller" or figure out if they're trying to use it anyway
        {
            ProcessMouseLook();
        }
        else
        {
            ProcessStickLook(device);
        }
    }

    private void ProcessStickLook(InputDevice device) //OPTION: Make a couple of layouts including one that moves everything to the face buttons
    {
        Vector2 inputLook = Vector2.zero;

        inputLook.x = device.RightStickY * mouseSensitivity;
        inputLook.y = device.RightStickX * mouseSensitivity;

        inputLook.x *= -1; //OPTION: Invert Look, inverting it doesn't invert it... so 

        ApplyLook(inputLook);
    }

    private void ProcessMouseLook()
    {
        Vector2 inputLook = Vector2.zero;

        inputLook.x = Input.GetAxis("Mouse Y") * mouseSensitivity;
        inputLook.y = Input.GetAxis("Mouse X") * mouseSensitivity;

        inputLook.x *= -1; //OPTION: Invert Look, inverting it doesn't invert it... so 

        ApplyLook(inputLook);
    }

    private void ApplyLook(Vector2 inputLook)
    {
        _camRotation.x += inputLook.x;
        _camRotation.y += inputLook.y;

        _camRotation.x = Mathf.Clamp(_camRotation.x, maxLookUp, maxLookDown);

        _playerRotation.y = _camRotation.y;

        _playerTransform.eulerAngles = _playerRotation;
        transform.eulerAngles = _camRotation;
    }
}
