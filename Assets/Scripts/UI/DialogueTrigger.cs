using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] [TextArea] string[] text;
    [SerializeField] string[] flair;
    [SerializeField] GameObject pceSource;

    List<IPostConvoEvent> events;

    private void Start()
    {
        IPostConvoEvent[] temp = new IPostConvoEvent[0];

        if (pceSource)
        {
            temp = pceSource.GetComponents<IPostConvoEvent>();
        }

        events = new List<IPostConvoEvent>(temp);
    }

    public bool TriggerDialogue(Transform obj)
    {
        if (text.Length != flair.Length)
        {
            Debug.LogError("Text-Flair length mismatch! Aborting dialogue trigger.");

            return true;
        }

        IPostConvoEvent pce = null;

        if (events.Count != 0)
        {
            pce = events[0];
            events.RemoveAt(0);
        }

        DialogueScript d = obj.GetComponent<DialogueScript>();

        d.enabled = true;

        d.Init(text, flair, GetInstanceID(), GetComponent<NPCAnimationBehavior>(), name, pce);

        return false; // stop camera from raycasting
    }

    public void SetText(string[] text, string[] flair, IPostConvoEvent[] events = null)
    {
        this.text = text;
        this.flair = flair;

        if (events != null)
        {
            this.events = new List<IPostConvoEvent>(events);
        }
    }
}
