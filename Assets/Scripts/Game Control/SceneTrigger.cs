using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void TriggerSceneLoad()
    {
        if (SceneController.instance)
        {
            SceneController.instance.ChangeScene(sceneName);
        }

        else
        {
            Debug.LogError("SceneController not found.");
        }
    }
}
