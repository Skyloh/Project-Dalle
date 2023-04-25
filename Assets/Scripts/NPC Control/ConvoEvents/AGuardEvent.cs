using UnityEngine;

// represents a convo event that bars progression to next event until predicate condition is false
public abstract class AGuardEvent : MonoBehaviour, IConvoEvent
{
    [SerializeField, TextArea] string[] guard_text;
    [SerializeField] string[] guard_flair;

    public void Activate(DialogueTrigger npcdt)
    {
        // pass
    }

    public bool Preactivate(ref string[] text, ref string[] flair)
    {
        if (Predicate())
        {
            text = guard_text;
            flair = guard_flair;

            return true;
        }

        return false;
    }

    protected abstract bool Predicate();
}
