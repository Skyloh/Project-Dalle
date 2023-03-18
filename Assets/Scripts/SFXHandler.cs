using System.Collections;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    [SerializeField] AudioClip footstep; // the audioclip of the footstep to play
    [SerializeField] float FOOTSTEP_SPEED = 35f; // how often the footstep thing should be played
    [SerializeField] PlayerDataSO data; // used to get the player's preferred volume
    static AudioSource source; // the audiosource used to play all sound effects

    float distance = 0f; // accumulator for how far the player has moved
    Vector3 prior; // the previous position. used in calculating delta

    int parity = 1; // swaps sound effect between left and right ear

    // Start:
    // assign an Audiosource to source, and the initial position to prior.
    private void Start()
    {
        source = GetComponent<AudioSource>();

        prior = transform.position;
    }

    // Update Cycle:
    // increment distance by the sqrDelta of distance multified by a constant to make it more usable
    // adjust prior to the correct value
    // if we've moved far enough, reset distance and play the footstep sound effect
    // this is to replicate a "stride"
    private void Update()
    {
        distance += (transform.position - prior).sqrMagnitude * FOOTSTEP_SPEED;

        prior = transform.position;

        if (distance > 1)
        {
            distance = 0;

            PlayFootstepSFX();
        }

    }

    // plays the given audioclip with half the volumescale.
    // if the audio should be scrambled, shifts the pitch by a random amount.
    public static void PlaySound(AudioClip c, bool scramble = false)
    {
        source.pitch = 1f;

        if (scramble)
        {
            source.pitch += Random.Range(-0.4f, 0.1f);
        }

        source.PlayOneShot(c, 0.5f);
    }

    // plays the footstep sound effect with a slightly varying pitch and pan at the
    // player's desired volume (given in PlayerDataSO).
    // The varying pitch and pan make the footsteps seem more real.
    void PlayFootstepSFX()
    {
        source.pitch = 1f;

        source.pitch += Random.Range(-0.25f, 0.75f);

        source.panStereo = 0.5f * parity;

        parity *= -1;

        source.PlayOneShot(footstep, data.VOLUME);
    }
}
