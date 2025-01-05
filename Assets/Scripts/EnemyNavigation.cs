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
    private bool isStunned = false;

    // Initializes the NavMeshAgent, sets the initial state to Patrol, and moves to the first patrol point
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = GameSettings.Instance.EnemySpeed;
        currentState = EnemyState.Patrol;
        GoToNextPatrolPoint();
    }

    // Handles enemy behavior, switching between chasing the player or patrolling based on the current state
    void Update()
    {
        //Debug.Log(currentState);

        if (isStunned) return;

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

    // Handles collisions with the player and pickup items, changing the state or triggering a stun effect as needed
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            setCurrentState(EnemyState.Patrol);
        }
        if (collision.gameObject.tag == "PickUp")
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(StunEnemy(2f));
        }
    }

    // Temporarily stuns the enemy, stopping its movement for a specified duration, and resumes its behavior afterward
    IEnumerator StunEnemy(float stunDuration)
    {
        isStunned = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        agent.isStopped = false;

        if (currentState == EnemyState.Patrol)
        {
            GoToNextPatrolPoint();
        }
    }

    // Directs the enemy to the next patrol point in the sequence, looping back to the start if needed
    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // Updates the enemy's current state and initiates the appropriate behavior for the new state
    public void setCurrentState(EnemyState state)
    {
        currentState = state;

        if (state == EnemyState.Patrol)
        {
            GoToNextPatrolPoint();
        }
    }
}
