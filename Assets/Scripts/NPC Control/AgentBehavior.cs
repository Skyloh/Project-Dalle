using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    NavMeshObstacle obstacle;
    NPCMoveBehavior movement;

    public bool reachedDestination = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        movement = GetComponent<NPCMoveBehavior>();

        agent.updateRotation = true;
    }

    public void SetDesination(Vector3 destination, Vector3 normal)
    {/*
        if (!reachedDestination)
        {
            Debug.LogWarning("NPC " + this.gameObject.name + " is already in process of moving.");

            return;
        }*/

        StopAllCoroutines();

        StartCoroutine(IECalculatePathAndStart(destination, normal));
    }

    public void SetAgentPause(bool paused)
    {
        agent.isStopped = paused;
    }

    public void SetAgentAsCarver()
    {
        obstacle.carving = true;

        Destroy(agent);
    }

    private IEnumerator IECalculatePathAndStart(Vector3 destination, Vector3 normal)
    {
        reachedDestination = false;

        FlipFlopNav(true);

        agent.SetDestination(destination);

        yield return new WaitUntil(() => agent.pathStatus == NavMeshPathStatus.PathComplete);

        movement.SetupPath(agent.path, normal);

        FlipFlopNav(false);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => !movement.isEnroute);

        reachedDestination = true;
    }

    public bool DidPathTerminateEarly()
    {
        return movement.wasPathCanceled;
    }


    void FlipFlopNav(bool is_agent_active)
    {
        agent.enabled = false;
        obstacle.enabled = false;

        agent.enabled = is_agent_active;
        obstacle.enabled = !is_agent_active;
    }
}
