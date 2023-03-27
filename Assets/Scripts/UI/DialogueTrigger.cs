using UnityEngine;
using System;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] [TextArea] string[] text;
    [SerializeField] string[] flair;

    public bool TriggerDialogue(Transform obj)
    {
        if (text.Length != flair.Length)
        {
            Debug.LogError("Text-Flair length mismatch! Aborting dialogue trigger.");

            return true;
        }

        DialogueScript d = obj.GetComponent<DialogueScript>();

        d.enabled = true;

        d.Init(text, flair, GetInstanceID(), GetComponent<NPCAnimationBehavior>());

        return false; // stop camera from raycasting
    }
}
