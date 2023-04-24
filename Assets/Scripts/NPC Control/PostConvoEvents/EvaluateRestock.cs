using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class EvaluateRestock : MonoBehaviour, IPostConvoEvent
{
    [SerializeField, TextArea] string[] text;
    [SerializeField] string[] flair;

    Regex regex;
    GameObject[] paintings;

    void Awake()
    {
        paintings = GameObject.FindGameObjectsWithTag("Painting");
        regex = new Regex("[0-9]+");
    }

    public void Activate(DialogueTrigger npcdt)
    {
        int score = (int)(new Grader().Grade(paintings) * 100);

        text[0] = regex.Replace(text[0], score.ToString());

        npcdt.SetText(text, flair, new IPostConvoEvent[1] { this });
    }

}
