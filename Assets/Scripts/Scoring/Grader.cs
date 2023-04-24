using UnityEngine;

public class Grader
{
    public float Grade(GameObject[] objs)
    {
        float sum = 0;
        int max = 0;

        foreach (GameObject obj in objs)
        {
            PaintingScript ps = obj.GetComponent<PaintingScript>();

            sum += ps.CalculateScore();

            max += ps.GetMaxScorePossible();
        }

        Debug.Log(sum + " " + max);

        return sum / max;
    }
}
