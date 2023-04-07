using System.Collections;
using UnityEngine;

public enum NPCStates
{
    Idle,
    Observing,
    Walking,
    Talking
}

public class NPCController : MonoBehaviour
{
    public BlendEmotions BlendShapeMood;

    [SerializeField] PlayerDataSO data;
    [SerializeField] bool LOCKED_TO_IDLE = false;
    [SerializeField] [Range(5f, 35f)] float observation_period = 5f;
    [SerializeField] Vector3[] positions_of_interest;
    Vector3 interest;

    NPCAnimationBehavior animBehavior;
    AgentBehavior agentBehavior;
    DialogueTrigger npcDialogueTrigger;

    SphereCollider interestTrigger;

    NPCStates state;
    NPCStates prior;

    IEnumerator process;

    private void Awake()
    {
        animBehavior = GetComponent<NPCAnimationBehavior>();
        agentBehavior = GetComponent<AgentBehavior>();
        npcDialogueTrigger = GetComponent<DialogueTrigger>();
        interestTrigger = GetComponent<SphereCollider>();

        interest = Vector3.zero;
    }

    private void Start()
    {
        if (!LOCKED_TO_IDLE)
        {
            state = NPCStates.Walking;

            process = IEPatrol();

            StartCoroutine(process);
        }

        else
        {
            state = NPCStates.Idle;

            animBehavior.PlayAnimation(Animations.Idle);
        }
    }

    void NextState(NPCStates next)
    {
        // exiting state change
        switch (state)
        {
            case NPCStates.Observing:
                interestTrigger.enabled = true;
                break;

            case NPCStates.Walking:
                // pass
                break;

            case NPCStates.Talking:
                agentBehavior.SetAgentPause(false);

                interestTrigger.enabled = true;
                break;

            default:
                // idle
                break;
        }

        if (process != null)
        {
            StopCoroutine(process);
        }

        prior = state;
        state = next;

        // incoming state change
        switch (next) 
        {
            case NPCStates.Observing:
                process = IEObserve();

                StartCoroutine(process);
                break;

            case NPCStates.Walking:
                process = IEPatrol();

                StartCoroutine(process);
                break;

            case NPCStates.Talking:
                process = IETalk();

                StartCoroutine(process);
                break;

            default:
                // idle
                animBehavior.PlayAnimation(Animations.Idle);
                break;
        }
    }

    public bool ProcessHit(Transform t)
    {
        NextState(NPCStates.Talking);

        // this is a bit messy since i just wanted NextState to call this, but
        // we can't pass the Transform any other way without adding stupid stuff.
        // this, by comparison, is less stupid.
        animBehavior.LookAt(t.position, AimTargetOps.Head);
        animBehavior.LookAt(t.position - Vector3.up * 0.33f, AimTargetOps.Chest); // offset slightly bc then the NPC looks up too much

        return npcDialogueTrigger.TriggerDialogue(t);
    }

    IEnumerator IEObserve()
    {
        interestTrigger.enabled = false;

        float time = 0f; // accumulates the time the NPC has been in an observing state

        float gaze_permanence = Random.Range(1f, 3f); // how long the NPC's gaze should rest on a position in seconds

        // gets a random offset for the NPC to "observe"
        Vector3 offset = transform.forward + Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1, 1f) * transform.right;

        // repeat this behavior while we are still observing
        while (time < observation_period)
        {
            // look towards the offsetted (scaled) interest point
            animBehavior.LookAt(interest + offset * 3f, AimTargetOps.Both);

            // randomly get a blend emotion to affect, then randomly set its weight
            BlendEmotions e = (BlendEmotions)(int)Random.Range(1f, 5f);
            animBehavior.SetBlendWeight(e, Random.Range(50f, 100f));


            yield return new WaitForSeconds(gaze_permanence); // wait the duration of the gaze before continuing

            time += gaze_permanence;  // increment time by how long we waited

            animBehavior.SetBlendWeight(e, 0f); // reset the affected blend weight

            // re-randomize values
            gaze_permanence = Random.Range(1f, 3f);
            offset = transform.forward + Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1, 1f) * transform.right;
        }

        // return to neutral
        ResetAnimBehavior();

        // give time for animBehavior to finish up its stuff
        yield return new WaitForSeconds(1f);

        NextState(NPCStates.Walking);
    }

    IEnumerator IEPatrol()
    {
        if (agentBehavior.reachedDestination || interest == Vector3.zero)
        {
            Vector3 destination = positions_of_interest[(int)Random.Range(0f, positions_of_interest.Length)];

            interest = destination;

            agentBehavior.SetDesination(destination);
        }

        animBehavior.PlayAnimation(Animations.Walk);

        yield return new WaitUntil(() => agentBehavior.reachedDestination);


        ResetAnimBehavior(AimTargetOps.Head);

        yield return new WaitForSeconds(1f);

        NextState(NPCStates.Observing);
    }

    IEnumerator IETalk()
    {
        interestTrigger.enabled = false;

        animBehavior.PlayAnimation(Animations.Idle);

        agentBehavior.SetAgentPause(true);

        // i need to wait a second here because there is a race condition.
        // data.IS_PLAYER_ENABLED is still true at this point, so I have to wait until
        // the DialogueTrigger finishes setting it to false.
        //
        // This is a hacky project :(
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => data.IS_PLAYER_ENABLED);

        ResetAnimBehavior();


        NextState(LOCKED_TO_IDLE ? NPCStates.Idle : prior);
    }

    void ResetAnimBehavior(AimTargetOps type = AimTargetOps.Reset)
    {
        animBehavior.ClearWeights();
        animBehavior.PlayAnimation(Animations.Idle);
        animBehavior.LookAt(Vector3.zero, type);
    }

    // while walking, if you see anything interesting, look at it.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Painting") || other.CompareTag("Player"))
        {
            animBehavior.LookAt(other.transform.position, AimTargetOps.Head);

            animBehavior.SetBlendWeight(BlendShapeMood, Random.Range(50f, 100f));
        }
    }

    // if you are standing still and the player comes to you, keep looking at them.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animBehavior.LookAt(other.transform.position, AimTargetOps.Head);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Painting") || other.CompareTag("Player"))
        {
            animBehavior.LookAt(Vector3.zero, AimTargetOps.Head);

            animBehavior.ClearWeights();
        }
    }
}
