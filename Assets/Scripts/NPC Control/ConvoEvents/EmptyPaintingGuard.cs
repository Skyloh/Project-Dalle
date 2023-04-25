using UnityEngine;

public class EmptyPaintingGuard : AGuardEvent
{
    PaintingScript[] paintings;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Painting");
        paintings = new PaintingScript[objs.Length];

        int i = 0;

        foreach (GameObject obj in objs)
        {
            paintings[i] = obj.GetComponent<PaintingScript>();

            i += 1;
        }
    }

    protected override bool Predicate()
    {
        // if there is an empty painting, return true
        foreach (PaintingScript ps in paintings)
        {
            if (ps.GetMaxScorePossible() == 0)
            {
                return true;
            }
        }

        return false;
    }
}
