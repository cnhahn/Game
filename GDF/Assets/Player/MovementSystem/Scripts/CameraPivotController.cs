using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera Pivot controller is used for crouching, that's so it is easier to overlay crouching onto the headbobbing. 
/// </summary>
public class CameraPivotController : MonoBehaviour
{
    [SerializeField]
    private float crouchHeight;
    [SerializeField]
    private float standingHeight;
    [SerializeField]
    private float crouchSpeed;

    private StateController _sc;

    private void Start()
    {
        SetupStateController();
    }

    private void Update()
    {
        ProcessCrouching();
    }

    private void SetupStateController()
    {
        _sc = transform.parent.GetComponent<StateController>();
    }

    private void ProcessCrouching()
    {
        Vector3 desiredPosition;

        if(_sc.state == StanceState.Crouching)
        {
            desiredPosition = new Vector3(0, crouchHeight, 0);
        }
        else
        {
            desiredPosition = new Vector3(0, standingHeight, 0);
        }

        transform.localPosition = Vector3.Lerp(this.transform.localPosition, desiredPosition, crouchSpeed);
    }
}
