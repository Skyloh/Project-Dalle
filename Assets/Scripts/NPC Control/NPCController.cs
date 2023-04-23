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
    int MoodIntensity;

    [SerializeField] PlayerDataSO data;
    [SerializeField] bool LOCKED_TO_IDLE = false;
    [SerializeField] [Range(5f, 35f)] float observation_period = 5f;
    [SerializeField] ObservationTransform[] positions_of_interest;
    ObservationTransform interest;

    int prior_interest = -1;

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

        MoodIntensity = (int)Random.Range(50f, 100f);
    }

    public void InitNPC(ObservationTransform[] ots, RuntimeAnimatorController rac, bool idle_locked)
    {
        LOCKED_TO_IDLE = idle_locked;

        if (idle_locked)
        {
            agentBehavior.SetAgentAsCarver();

            Destroy(GetComponent<NPCMoveBehavior>());
        }

        animBehavior.SetRuntimeAnimator(rac);

        positions_of_interest = ots;
        

        if (!LOCKED_TO_IDLE)
        {
            PickAndSet();

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

    void PickAndSet()
    {
        int new_interest;

        do
        {
            new_interest = (int)Random.Range(0f, positions_of_interest.Length);

        } while (prior_interest == new_interest);

        interest = positions_of_interest[new_interest];

        prior_interest = new_interest;

        Vector3 dest = interest.world_position + interest.painting_normal * 3f + Random.insideUnitSphere;
        dest.y = transform.position.y;

        agentBehavior.SetDesination(dest, interest.painting_normal);
    }

    void NextState(NPCStates next)
    {
        // exiting state change
        switch (state)
        {
            case NPCStates.Observing:
                interestTrigger.enabled = true;
                animBehavior.ClearWeights();
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
        if (ShouldIgnoreObject(t))
        {
            // if we try to talk to them from behind, ignore that
            return true;
        }

        NextState(NPCStates.Talking);

        // this is a bit messy since i just wanted NextState to call this, but
        // we can't pass the Transform any other way without adding stupid stuff.
        // this, by comparison, is less stupid.
        animBehavior.LookAt(t.position, AimTargetOps.Head);
        animBehavior.LookAt(t.position - Vector3.up * 0.33f, AimTargetOps.Chest); // offset slightly bc then the NPC looks up too much

        return npcDialogueTrigger.TriggerDialogue(t);
    }

    bool ShouldIgnoreObject(Transform o)
    {
        Vector3 other = o.position;
        other.y = 0;

        other -= transform.position;

        return Vector3.Dot(transform.forward, other) < -0.2f * (interestTrigger.radius * 2f);
    }

    IEnumerator IEObserve()
    {
        interestTrigger.enabled = false;

        float time = 0f; // accumulates the time the NPC has been in an observing state

        float gaze_permanence = Random.Range(1f, 3f); // how long the NPC's gaze should rest on a position in seconds

        // gets a random offset for the NPC to "observe"
        Vector3 offset = 
            interest.painting_normal 
            + Random.Range(-1f, 1f) * interest.max_observational_area.y * Vector3.up 
            + Random.Range(-1, 1f) 
            * interest.max_observational_area.x 
            * Vector3.Cross(transform.position - interest.world_position, Vector3.up);

        // repeat this behavior while we are still observing
        while (time < observation_period)
        {
            // look towards the offsetted (scaled) interest point
            animBehavior.LookAt(interest.world_position + offset, AimTargetOps.Both);

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
        if (agentBehavior.reachedDestination)
        {
            PickAndSet();
        }

        animBehavior.PlayAnimation(Animations.Walk);

        yield return new WaitUntil(() => agentBehavior.reachedDestination);

        ResetAnimBehavior(AimTargetOps.Head);

        yield return new WaitForSeconds(1f);

        NPCStates next;

        if (agentBehavior.DidPathTerminateEarly())
        {
            // wait for a moment, then repath and continue
            animBehavior.PlayAnimation(Animations.Idle);

            yield return new WaitForSeconds(Random.Range(0.1f, 1.5f));

            next = NPCStates.Walking;
        }

        else
        {
            next = NPCStates.Observing;
        }

        NextState(next);
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
        //animBehavior.SetBlendWeight(BlendShapeMood, 0f);
        animBehavior.PlayAnimation(Animations.Idle);
        animBehavior.LookAt(Vector3.zero, type);
    }

    // while walking, if you see anything interesting, look at it.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Painting") || other.CompareTag("Player"))
        {
            animBehavior.LookAt(other.transform.position + Vector3.up * 0.25f, AimTargetOps.Head);

            animBehavior.SetBlendWeight(BlendShapeMood, MoodIntensity);
        }
    }

    // if you are standing still and the player comes to you, keep looking at them.
    private void OnTriggerStay(Collider other)
    {
        if (!ShouldIgnoreObject(other.transform) && (other.CompareTag("Painting") || other.CompareTag("Player")))
        {
            animBehavior.LookAt(other.transform.position + Vector3.up * 0.25f, AimTargetOps.Head);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Painting") || other.CompareTag("Player"))
        {
            animBehavior.LookAt(Vector3.zero, AimTargetOps.Head);

            animBehavior.SetBlendWeight(BlendShapeMood, 0f);
        }
    }
}
