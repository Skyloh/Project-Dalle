using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    NPCAnimationBehavior animBehavior;
    AgentBehavior agentBehavior;

    [SerializeField] Vector3[] positions_of_interest;

    IEnumerator walkProcess;

    private void Awake()
    {
        animBehavior = GetComponent<NPCAnimationBehavior>();
        agentBehavior = GetComponent<AgentBehavior>();
    }

    private void Start()
    {
        walkProcess = IEPatrol();

        StartCoroutine(walkProcess);
    }

    IEnumerator IEObserve()
    {
        yield return null;
    }

    IEnumerator IEPatrol()
    {
        Vector3 destination = positions_of_interest[(int)Random.Range(0f, positions_of_interest.Length)];

        agentBehavior.SetDesination(destination);
        animBehavior.PlayAnimation(Animations.Walk);

        yield return new WaitUntil(() => agentBehavior.reachedDestination);

        animBehavior.PlayAnimation(Animations.Idle);


        animBehavior.LookAt(Vector3.zero, AimTargetOps.Head);

        animBehavior.ClearWeights();

        // change state to observing
    }

    // while walking, if you see anything interesting, look at it.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Painting"))
        {
            animBehavior.LookAt(other.transform.position, AimTargetOps.Head);

            animBehavior.SetBlendWeight((BlendEmotions)(int)Random.Range(0f, 5f), Random.Range(50f, 100f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Painting"))
        {
            animBehavior.LookAt(Vector3.zero, AimTargetOps.Head);

            animBehavior.ClearWeights();
        }
    }
}
