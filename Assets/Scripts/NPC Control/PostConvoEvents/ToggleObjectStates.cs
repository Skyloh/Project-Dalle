using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectStates : MonoBehaviour, IPostConvoEvent
{
    [SerializeField] List<GameObject> objects;
    [SerializeField] bool state;

    public void Activate(DialogueTrigger npcdt)
    {
        foreach(GameObject g in objects)
        {
            g.SetActive(state);
        }
    }
}
