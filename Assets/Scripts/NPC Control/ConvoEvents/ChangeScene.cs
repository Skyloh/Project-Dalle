using UnityEngine;

public class ChangeScene : MonoBehaviour, IConvoEvent
{
    [SerializeField] string sceneName;

    [SerializeField, TextArea] string[] mutate_text;
    [SerializeField] string[] mutate_flair;

    public virtual bool Preactivate(ref string[] t, ref string[] f)
    {
        if (mutate_text.Length != 0)
        {
            t = mutate_text;
            f = mutate_flair;
        }

        return false;
    }

    public void Activate(DialogueTrigger npcdt)
    {
        SceneController.instance.ChangeScene(sceneName);
    }
}
