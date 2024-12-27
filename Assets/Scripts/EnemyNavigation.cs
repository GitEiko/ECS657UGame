using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.speed = GameSettings.Instance.EnemySpeed;
        currentState = EnemyState.Patrol;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        //Debug.Log(currentState);
        if (currentState == EnemyState.Chase)
        {
            gameObject.transform.rotation.Equals(player.rotation);
            agent.destination = player.position;
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPatrolPoint();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            setCurrentState(EnemyState.Patrol);
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    public void setCurrentState(EnemyState state)
    {
        currentState = state;

        if (state == EnemyState.Patrol)
        {
            GoToNextPatrolPoint();
        }
    }
}
