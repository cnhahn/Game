using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private GameObject _player;
    private NavMeshAgent _agent;
    private LightLevel _lightLevel;
    private GameObject _safeZone;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _safeZone = GameObject.FindGameObjectWithTag("MonsterSafeZone");
        _lightLevel = _player.GetComponent<LightLevel>();
        _agent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_lightLevel.isSafe)
        {
            if (_agent.isStopped == true)
            {
                _agent.isStopped = false;
                _agent.destination = _safeZone.transform.position;
            }

            _agent.destination = _safeZone.transform.position;
        }
        else
        {
            if (_agent.isStopped == true)
            {
                _agent.isStopped = false;
                _agent.destination = _player.transform.position;
            }

            _agent.destination = _player.transform.position;
        }

        if(Vector3.Distance(this.transform.position, _player.transform.position) < 1)
        {
            _player.GetComponent<Oxygen>().KillPlayer();
        }

        if (Vector3.Distance(this.transform.position, _player.transform.position) < 5)
        {
            float input = Vector3.Distance(this.transform.position, _player.transform.position);
            float min = 1;
            float max = 5;

            float val = 100 - (((input - min) * 100) / (max - min));

            _player.GetComponent<Oxygen>().DyingEffects(val / 2);
        }
    }
}
