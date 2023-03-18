using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentBehavior : MonoBehaviour
{
    NavMeshAgent agent;

    public bool reachedDestination = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = true;
    }

    public void SetDesination(Vector3 destination)
    {
        reachedDestination = false;

        StopAllCoroutines();

        agent.SetDestination(destination);

        StartCoroutine(IEWaitTill());
    }

    private IEnumerator IEWaitTill()
    {
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => agent.velocity.sqrMagnitude < 2f);

        reachedDestination = true;
    }
}
