using System.Collections;
using UnityEngine;

public class SFXHandler : MonoBehaviour
{
    [SerializeField] PlayerDataSO data; // used to get the player's preferred volume
    AudioSource source; // the audiosource used to play all sound effects

    int parity = 1; // swaps sound effect between left and right ear

    float original_pitch;

    // Start:
    // assign an Audiosource to source, and the initial position to prior.
    private void Start()
    {
        source = GetComponent<AudioSource>();

        original_pitch = source.pitch;
    }

    // plays the given audioclip with half the volumescale.
    // if the audio should be scrambled, shifts the pitch by a random amount.
    public void PlaySound(AudioClip c, bool scramble = false)
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
    public void PlayFootstepSFX(AudioClip footstep, bool do_pan = false)
    {
        source.pitch = original_pitch + Random.Range(-0.25f, 0.75f);

        if (do_pan)
        {
            source.panStereo = 0.5f * parity;
        }

        else
        {
            source.panStereo = 0f;
        }

        parity *= -1;

        source.PlayOneShot(footstep, data.VOLUME);
    }
}
