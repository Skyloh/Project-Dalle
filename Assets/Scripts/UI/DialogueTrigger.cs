using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] [TextArea] string[] text;
    [SerializeField] string[] flair;
    [SerializeField] GameObject pceSource;

    List<IConvoEvent> events;

    private void Start()
    {
        IConvoEvent[] temp = new IConvoEvent[0];

        if (pceSource)
        {
            temp = pceSource.GetComponents<IConvoEvent>();
        }

        events = new List<IConvoEvent>(temp);
    }

    public bool TriggerDialogue(Transform obj)
    {
        string[] ntext = text;
        string[] nflair = flair;

        IConvoEvent pce = null;

        if (events.Count != 0)
        {
            pce = events[0];

            // if the event doesn't a pre-effect, remove it when used.
            if (!pce.Preactivate(ref ntext, ref nflair))
            {
                events.RemoveAt(0);
            }
        }

        if (ntext.Length != nflair.Length)
        {
            Debug.LogError("Text-Flair length mismatch! Aborting dialogue trigger.");

            return true;
        }

        DialogueScript d = obj.GetComponent<DialogueScript>();

        d.enabled = true;

        d.Init(ntext, nflair, GetInstanceID(), GetComponent<NPCAnimationBehavior>(), name, pce);

        return false; // stop camera from raycasting
    }

    public void SetText(string[] text, string[] flair, IConvoEvent[] events = null)
    {
        this.text = text;
        this.flair = flair;

        if (events != null)
        {
            this.events = new List<IConvoEvent>(events);
        }
    }
}
