using UnityEngine;

public class FootstepEmitter : MonoBehaviour
{
    [SerializeField] AudioClip footstep; // the audioclip of the footstep to play
    [SerializeField] float FOOTSTEP_SPEED; // how often the footstep thing should be played
    [SerializeField] bool do_pan = false;

    SFXHandler sfx;

    float distance = 0f; // accumulator for how far the player has moved
    Vector3 prior; // the previous position. used in calculating delta


    private void Awake()
    {
        sfx = GetComponent<SFXHandler>();
    }

    private void Start()
    {
        prior = transform.position;
    }

    // Update Cycle:
    // increment distance by the sqrDelta of distance multified by a constant to make it more usable
    // adjust prior to the correct value
    // if we've moved far enough, reset distance and play the footstep sound effect
    // this is to replicate a "stride"
    private void Update()
    {
        distance += (Time.deltaTime * FOOTSTEP_SPEED * (transform.position - prior)).sqrMagnitude;

        prior = transform.position;

        if (distance > 1)
        {
            distance = 0;

            sfx.PlayFootstepSFX(footstep, do_pan);
        }

    }
}
