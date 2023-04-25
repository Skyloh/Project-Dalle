using UnityEngine;

public class ChangeScene : MonoBehaviour, IConvoEvent
{
    [SerializeField] string sceneName;

    public bool Preactivate(ref string[] _none, ref string[] __none)
    {
        return false;
    }

    public void Activate(DialogueTrigger npcdt)
    {
        SceneController.instance.ChangeScene(sceneName);
    }
}
