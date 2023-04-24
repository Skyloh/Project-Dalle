using UnityEngine;

[System.Serializable]
public class DialoguePack
{
    [TextArea] public string[] dialogue;
    public string[] flair;
    public GameObject source;
}
