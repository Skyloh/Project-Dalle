using UnityEngine;

public class ToggleObjects : MonoBehaviour, IConvoEvent
{
    [SerializeField] GameObject[] objects;
    [SerializeField] bool state = false;

    public void Activate(DialogueTrigger npcdt)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(state);
        }
    }

    public bool Preactivate(ref string[] text, ref string[] flair)
    {
        return false;
    }
}
