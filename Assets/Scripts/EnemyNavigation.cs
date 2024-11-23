using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent enemy;

    void Start()
    {
        enemy = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        gameObject.transform.rotation.Equals(player.rotation);
        enemy.destination = player.position;
    }
}
