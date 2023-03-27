using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IRaycastable
{
    [SerializeField] [TextArea] string[] text;

    public bool OnHit(Transform obj)
    {
        DialogueScript d = obj.GetComponent<DialogueScript>();

        d.enabled = true;

        d.Init(text, GetInstanceID());

        return false; // stop camera from raycasting
    }
}
