using UnityEngine;
using System.Text.RegularExpressions;

public class BranchingEvaluateRestock : MonoBehaviour, IConvoEvent
{
    [SerializeField, TextArea] string[] high_text;
    [SerializeField] string[] high_flair;
    [SerializeField, TextArea] string[] mid_text;
    [SerializeField] string[] mid_flair;
    [SerializeField, TextArea] string[] low_text;
    [SerializeField] string[] low_flair;
    [SerializeField] int score_index;

    bool finished = false;

    Regex regex;
    GameObject[] paintings;

    void Awake()
    {
        paintings = GameObject.FindGameObjectsWithTag("Painting");
        regex = new Regex("[-0-9]+");
    }

    public bool Preactivate(ref string[] t, ref string[] f)
    {
        int score = (int)(new Grader().Grade(paintings) * 100);

        string[] text;
        string[] flair;

        if (score > 80)
        {
            text = high_text;
            flair = high_flair;

            finished = true;
        }

        else if (score > 50)
        {
            text = mid_text;
            flair = mid_flair;
        }

        else
        {
            text = low_text;
            flair = low_flair;
        }

        text[score_index] = regex.Replace(text[score_index], score.ToString());

        t = text;
        f = flair;

        return !finished;
    }

    public void Activate(DialogueTrigger npcdt)
    {
        // pass
    }

}
