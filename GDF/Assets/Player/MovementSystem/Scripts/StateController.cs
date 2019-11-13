using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State controller is a simple way to add states like prone etc to the game
/// Nothing fancy really but keeps it separate
/// </summary>
public class StateController : MonoBehaviour
{
    public float speed;
    public StanceState state;
}

public enum StanceState
{
    Standing,
    Crouching
}
