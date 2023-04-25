using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTestEvent : MonoBehaviour, IConvoEvent
{
    public void Activate(DialogueTrigger npcdt)
    {
        // pass
    }

    public bool Preactivate(ref string[] text, ref string[] flair)
    {
        if (Random.Range(0f, 5f) > 2f)
        {
            text = new string[2] { "No.", "Nope." };
            flair = new string[2] { "gesture", "idleflair" };

            return true;
        }


        text = new string[1] { "K" };
        flair = new string[1] { "emote happy 10" };

        return false;
    }
}
