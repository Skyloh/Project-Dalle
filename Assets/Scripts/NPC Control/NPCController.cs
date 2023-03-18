using System.Collections;
using UnityEngine;

public enum NPCStates
{
    Observing,
    Walking,
    Talking
}

public class NPCController : MonoBehaviour
{
    NPCAnimationBehavior animBehavior;
    AgentBehavior agentBehavior;

    NPCStates state;

    [SerializeField] [Range(5f, 35f)] float observation_period = 5f;

    [SerializeField] Vector3[] positions_of_interest;
    Vector3 interest;

    IEnumerator process;

    private void Awake()
    {
        animBehavior = GetComponent<NPCAnimationBehavior>();
        agentBehavior = GetComponent<AgentBehavior>();
    }

    private void Start()
    {
        state = NPCStates.Walking;

        process = IEPatrol();

        StartCoroutine(process);
    }

    void NextState(NPCStates next)
    {
        if (process != null)
        {
            StopCoroutine(process);
        }

        state = next;

        switch (next) 
        {
            case NPCStates.Observing:
                process = IEObserve();
                break;

            case NPCStates.Walking:
                process = IEPatrol();
                break;

            case NPCStates.Talking:
                // pass
                break;

            default:
                // pass
                break;
        }

        StartCoroutine(process);
    }

    IEnumerator IEObserve()
    {
        // TODO clean up this method. It's quite messy.
        // comment it, too.

        float time = 0f;

        float value = Random.Range(1f, 3f);
        Vector3 offset = transform.forward + Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1, 1f) * transform.right;

        while (time < observation_period)
        {
            animBehavior.LookAt(interest + offset * 3f, AimTargetOps.Both);

            BlendEmotions e = (BlendEmotions)(int)Random.Range(1f, 5f);

            animBehavior.SetBlendWeight(e, Random.Range(50f, 100f));

            yield return new WaitForSeconds(value);

            time += value;

            animBehavior.SetBlendWeight(e, 0f);

            value = Random.Range(1f, 3f);

            offset = transform.forward + Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1, 1f) * transform.right;
        }

        animBehavior.ClearWeights();
        animBehavior.PlayAnimation(Animations.Idle);
        animBehavior.LookAt(Vector3.zero, AimTargetOps.Reset);

        yield return new WaitForSeconds(1f);

        NextState(NPCStates.Walking);
    }

    IEnumerator IEPatrol()
    {
        Vector3 destination = positions_of_interest[(int)Random.Range(0f, positions_of_interest.Length)];

        interest = destination;

        agentBehavior.SetDesination(destination);
        animBehavior.PlayAnimation(Animations.Walk);

        yield return new WaitUntil(() => agentBehavior.reachedDestination);


        animBehavior.ClearWeights();
        animBehavior.PlayAnimation(Animations.Idle);
        animBehavior.LookAt(Vector3.zero, AimTargetOps.Head);

        yield return new WaitForSeconds(1f);

        NextState(NPCStates.Observing);
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
