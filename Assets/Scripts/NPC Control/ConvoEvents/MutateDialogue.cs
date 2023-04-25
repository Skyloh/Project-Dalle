using UnityEngine;

public class MutateDialogue : MonoBehaviour, IConvoEvent
{
    [SerializeField, TextArea] string[] text;
    [SerializeField] string[] flair;

    public void Activate(DialogueTrigger npcdt)
    {
        // pass
    }

    public bool Preactivate(ref string[] text, ref string[] flair)
    {
        text = this.text;
        flair = this.flair;

        return false;
    }
}
