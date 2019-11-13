using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Headbob script for the player, kept separate to make it easy to reuse or disable.
/// The script uses a simple sine wave for the headbobbing. Cheap, Simple and Effective. 
/// </summary>
public class HeadbobShake : MonoBehaviour
{
    [SerializeField]
    private float idleAmplitude = 0.8f;
    [SerializeField]
    private float idlePeriod = 0.5f;
    [SerializeField]
    private float moveAmplitude = 0.8f;
    [SerializeField]
    private float movePeriod = 0.3f;
    [SerializeField]
    private float smoothing = 0.4f;

    private StateController _sc;

    //=====
    //Unity Functions
    //=====
    private void Start()
    {
        SetupStateController();
    }

    private void Update()
    {
        ProcessHeadbob();
    }

    //=====
    //Private Functions
    //=====

    private void SetupStateController()
    {
        _sc = FindObjectOfType<StateController>();
    }

    private void ProcessHeadbob()
    {
        if (_sc.speed > 4)
        {
            float theta = Time.timeSinceLevelLoad / movePeriod;
            float distance = moveAmplitude * Mathf.Sin(theta);
            transform.position = Vector3.Lerp(this.transform.position, transform.parent.position + Vector3.up * distance, smoothing);
        }
        else
        {
            float theta = Time.timeSinceLevelLoad / idlePeriod;
            float distance = idleAmplitude * Mathf.Sin(theta);
            transform.position = Vector3.Lerp(this.transform.position, transform.parent.position + Vector3.up * distance, smoothing);
        }
    }
}
