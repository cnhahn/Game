using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private GameObject _player;
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent.isStopped == true)
        {
            _agent.isStopped = false;
            _agent.destination = _player.transform.position;
        }

        _agent.destination = _player.transform.position;
    }
}
