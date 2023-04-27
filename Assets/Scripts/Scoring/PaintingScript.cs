using UnityEngine;
using TMPro;

public class PaintingScript : MonoBehaviour, IRaycastable
{
    [SerializeField] PaintingSO paintingSO;
    [SerializeField] PlacardSO placardSO;
    [SerializeField] SpriteRenderer painting;
    [SerializeField] TextMeshPro placardText;

    int hashCode;

    void Start()
    {
        placardText.text = placardSO.description;

        if (paintingSO)
        {
            painting.sprite = paintingSO.painting;
        }

        hashCode = GetHashCode();
    }

    public bool OnHit(Transform source)
    {
        // source is Camera, and Camera has this component.
        // I didn't want to make it static, so i just did this instead.
        SelectionCanvasScript s = source.GetComponent<SelectionCanvasScript>();
        
        s.StartCanvas(this);

        return false; // stop camera raycasting
    }

    public void SetPainting(PaintingSO p)
    {
        if (paintingSO)
        {
            paintingSO.RemoveIDFromList(hashCode);
        }

        p.AddIDToList(hashCode);

        paintingSO = p;

        painting.sprite = p.painting;
    }

    public bool IsIdentical(PaintingSO other)
    {
        return other.high_keywords.Equals(paintingSO.high_keywords);
    }

    public string GetCurrentPlacardText()
    {
        return placardSO.description;
    }

    public void SetPlacard(PlacardSO p)
    {
        placardSO = p;

        placardText.text = placardSO.description;
    }

    public int CalculateScore()
    {
        return paintingSO.Score(placardSO.GetWords());
    }

    public int GetMaxScorePossible()
    {
        return paintingSO.MaxScore();
    }
}
