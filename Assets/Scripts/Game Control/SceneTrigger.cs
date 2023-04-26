using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

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
