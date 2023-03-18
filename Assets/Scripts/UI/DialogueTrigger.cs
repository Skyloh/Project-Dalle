using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IRaycastable
{
    [SerializeField] [TextArea] string[] text;

    public void OnHit(Transform obj)
    {
        DialogueScript d = obj.GetComponent<DialogueScript>();

        string[] patched = new string[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            patched[i] = " " + text[i];
        }

        d.Init(text, GetInstanceID());
    }
}
