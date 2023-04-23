using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour, IPostConvoEvent
{
    [SerializeField] string sceneName;

    public void Activate(DialogueTrigger npcdt)
    {
        SceneController.instance.ChangeScene(sceneName);
    }
}
